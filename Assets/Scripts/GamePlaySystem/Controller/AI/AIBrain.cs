using System.Collections.Generic;
using BattleSystem.FactionSystem;
using Entity;
using Entity.Unit;
using GamePlaySystem.Controller.AI.AIDecisionResource;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public class AIBrain : IBrain
    {
        private readonly CharacterManager characterManager;
        private readonly TileManager _tileManager;
        private readonly NavigationService navigationService;
        private readonly ScoreHeatMapCooker scoreHeatMapCooker;

        // 交由战斗管理器注入
        public AIBrain(CharacterManager characterManager, TileManager tileManager,
            NavigationService navigationService)
        {
            this.characterManager = characterManager;
            _tileManager = tileManager;
            this.navigationService = navigationService;
            scoreHeatMapCooker = new ScoreHeatMapCooker(this.characterManager, _tileManager);
        }

        private const float ScoreEnoughToAct = 100f;
        private const float KillHostileScore = 50f;
        private const float KillFriendlyScore = -50f;

        private float[,] _riskMap;
        private Character _decider;
        private Transform _deciderTrans;
        private List<SkillSlot> _deciderSkills;
        private FactionType _hostileFaction;
        private List<Character> _hostiles;

        private readonly Queue<ICommand> _actions = new();

        /// <summary>
        /// 尽量返回当前决策层下的最高得分的决策
        /// </summary>
        private void DoTactic(Character decider, int remainActionCnt, int rwr, int ap, int sp, Vector2 virtualPos)
        {
            bool hasAnySkillReady = false; // 如果是，则角色进入最终移动距离
            bool hasAnyHostileInReadyAttackRange = false; // 如果是，则角色先移动一段距离，再进行决策。
            // 角色还能做出除了移动以外的动作
            while (rwr > 0 && remainActionCnt > 0 && ap > 0) // 没有行动资源了 // sp <= 0 一般不判断SP，SP几乎很少单独使用
            {
                // 这是攻击阶段的循环，分为攻击判断阶段以及距离不足时的向前靠近阶段
                BaseSkill skillChosen = null;
                // TODO 如果添加AOE支持，需要将其改为List
                var targetsChosen = new List<BaseEntity>();
                float maxActionScore = 0;
                foreach (var slot in _deciderSkills)
                {
                    var skill = slot.skill;
                    // 检查技能是否可释放
                    if (slot.RemainCoolDown.Value > 0 || skill.AP_Cost > ap || skill.SP_Cost > sp) continue; 
                    
                    hasAnySkillReady = true;
                    // 如果是伤害技能
                    if (skill is DirectedDamageSkill damageSkill)
                    {
                        switch (damageSkill.impactType)
                        {
                            case ImpactType.Single:
                                // TODO 获取攻击范围格子
                                foreach (var hostile in _hostiles)
                                {
                                    if (!FactionManager.CanImpact(_decider, hostile, skill.canImpactFactionType)) continue;
                                    // 技能能否命中敌人
                                    if (damageSkill.isTargetInATKRange(
                                            virtualPos,
                                            hostile.transform.position))
                                    {
                                        hasAnyHostileInReadyAttackRange = true;
                                        var actionScore = GetAttackScore(damageSkill, hostile);
                                        // 如果得分高于最高得分，则保存此动作
                                        if (actionScore > maxActionScore)
                                        {
                                            maxActionScore = actionScore;
                                            // 生成动作命令，考虑对象池
                                            skillChosen = damageSkill;
                                            targetsChosen.Clear();
                                            targetsChosen.Add(hostile);
                                        }
                                    }
                                }
                                break;
                            // 退一步，AOE技能只会瞄准敌人来减少遍历，不再对范围内所有格子释放(即使有可能得到更高的得分)
                            case ImpactType.Aoe:
                                break;
                        }
                    }
                }
                // 如果有技能可以释放
                if (skillChosen != null)
                {
                    // TODO 需要重新设置技能的冷却
                    _actions.Enqueue(GetSkillCommand(skillChosen, decider, targetsChosen.ToArray()));
                    // 计算消耗
                    ap -= skillChosen.AP_Cost;
                    sp -= skillChosen.SP_Cost;
                    remainActionCnt--;
                }
                // 如果是因为距离不够导致没有合适的技能释放, 则考虑前进一段距离，再次递归技能，否则寻找最佳位置结束回合。
                else if (hasAnySkillReady && hasAnyHostileInReadyAttackRange) {
                    // 如果是因为距离原因导致没有合适的技能释放, 则考虑朝某个角色移动一段距离，再次递归技能，否则寻找最佳位置结束回合。
                    // 获取可到达的地格
                    var pathNodesDict = navigationService.GetReachablePositionDict(
                        (int)_deciderTrans.position.x,
                        (int)_deciderTrans.position.y,
                        rwr + 1); // +1 是因为决策者走到攻击范围内即可。
                    // 先判断有没有敌人处于移动范围内的地格，如果有直接向前移动一段随机距离，再进行攻击决策。
                    foreach (var hostile in _hostiles)
                    {
                        var hostilePos = hostile.transform.position;
                        if (!pathNodesDict.TryGetValue( 
                                NavigationService.GetPathNodeKey((int)hostilePos.x, (int)hostilePos.y), out var endNode)) 
                            continue;
                        // 敌人在可到达路径内
                        var moveDist = Random.Range(1, rwr + 1);
                        // 可以理解为路径上不存在其它单位（除了终点位置和起点位置）（由导航系统确保）
                        // 从终点位置开始向前移动一个，即离敌人最近，消耗最少的地格(到达此格即可进行攻击)，所以目标点是终点的前一格
                        var destNode = endNode.FromNode; 
                        // 尽可能的消耗随机距离所指定的步数
                        while (moveDist < destNode.Cost && destNode.FromNode != null) destNode = destNode.FromNode;
                        // 距离不够，换个敌对目标继续判断
                        if (destNode.Cost > moveDist) continue;
                        
                        // 【生成动作命令】，考虑对象池
                        var followPathCommand = new FollowPathCommand();
                        followPathCommand.Init(decider, destNode);
                        _actions.Enqueue(followPathCommand);
                        // 【更新参数】
                        rwr -= destNode.Cost;
                        virtualPos = new Vector2Int(destNode.PosX, destNode.PosY);
                    }
                }
                else
                {
                    // 没有行动资源，结束回合
                    break;
                }
            }
            
            // 角色行动阶段，利用剩余行动资源移动至得分最高的地格。
            if (rwr > 0)
            {
                float maxScore = 0;
                // 获取可到达的地格
                var pathNodesDict = navigationService.GetReachablePositionDict(
                    (int) virtualPos.x, (int) virtualPos.y, rwr);
                
                // 根据危险热力图决定分数
                var bestPos = new Vector2Int();
                // 遍历所有可到达的地格,取得分最高的地格。
                foreach (var pathNode in pathNodesDict)
                {
                    var pos = new Vector2Int(pathNode.Value.PosX, pathNode.Value.PosY);
                    var score = _riskMap[pos.x, pos.y];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestPos = pos;
                    }
                }
                // 【生成动作命令】，考虑对象池
                var followPathCommand = new FollowPathCommand();
                followPathCommand.Init(decider, pathNodesDict[NavigationService.GetPathNodeKey(bestPos.x, bestPos.y)]);
                _actions.Enqueue(followPathCommand);
            }
        }
        
        private float GetAttackScore(DirectedDamageSkill skill, Character target)
        {
            var damage = skill.damage;
            var score = (float)damage;
            if (target.property.HP.Value <= damage)
            {
                score += KillHostileScore;
                // TODO 如果性能允许，则更新危险热力图
            }

            return score;
        }

        private BaseSkillCommand GetSkillCommand(BaseSkill skill, Character caster, BaseEntity[] targets)
        {
            var skillCommand = new BaseSkillCommand();
            skillCommand.Init(skill, caster, targets);
            return skillCommand;
        }

        public Queue<ICommand> DoTactics(Character decisionMaker)
        {
            _actions.Clear();
            // 初始化
            _decider = decisionMaker;
            _deciderTrans = _decider.transform;
            _deciderSkills = decisionMaker.Skills;
            _hostileFaction = (1 - decisionMaker.Faction.Value); // TODO 只当仅有两个阵营此代码有效
            _hostiles = characterManager.GetUnitsByFaction(_hostileFaction);
            // TODO AI逻辑

            // 1、获取战场地图格子
            var tiles = _tileManager.GetTiles();
            // 2、绘制危险区域
            _riskMap = scoreHeatMapCooker.Cook(_decider);
            // 3、递归策略函数得到最佳决策结果
            var prop = _decider.property;
            DoTactic(_decider, 1, prop.RWR.Value, prop.AP.Value, prop.SP.Value, _deciderTrans.position);
            // 4、执行决策
            return _actions;

            // 角色行动资源【RWR，AP，SP】
            // 【策略函数 - HP状态正常】
            // Dequeue<Command> actions (用于描述角色的行动，注意池化)
            // int GetTacticCommand(Character char, int curScore)
            // 此函数返回当前行动节点所有行动的最高得分，跟据行动往actions中添加行动 

            // 1、是否有剩余行动资源，如果没有则结束回合
            // 1、遍历所有技能（如果可释放，则进入以下判断）
            // - 1.1、如果是指向性伤害技能，依据（移动范围加攻击范围）作为射程，如果能攻击到敌人，则依照【攻击得分策略】计算得分，（扣除相应消耗）进入下一步决策。
            // - 1.2、如果是范围伤害技能，依据（移动范围加攻击范围）作为射程，遍历所有地格得到受影响的有效影响对象(敌人单位数量减去友军单位数量)
            // 然后如果有效影响对象<=0，则跳过该地格，然后各有效影响对象依次依照【攻击得分策略】计算得分，（扣除相应消耗）进入下一步决策。

            // 【攻击得分策略】
            // 移动结束时，根据所处地格的危险度加减分
            // 攻击结束时，如果攻击目标是敌人，则根据伤害值*系数加分
            // 如果攻击导致敌人死亡，则额外加分
            // 如果攻击导致友军受伤，则根据伤害值*系数扣分
            // 如果攻击导致友军死亡，则根据伤害值*系数扣大分
            // 【治疗得分策略】
            // 如果治疗技能目标是友军，则根据 治疗值 * 系数 加分（治疗技能不能选中敌人，但不排除范围治疗）
            // 如果治疗技能目标是敌人，则根据 治疗值 * 系数 扣分
            //【控制得分策略】 待定，说起来博德之门3的AI也不会创造地形，只会利用地形站位

            // 【区域停留选择策略】
            // 计算敌人的危险度【血量 + 攻击力】减分 // 也可通过固定值代替威胁度 或者去掉血量的影响，以便减少刷新次数。
            // 每个敌人都会对周围区域造成威胁，施加的减分随着距离的增加而衰减
            // 每个友方角色的一定范围内都会有一个安全区域，安全区域的大小取决于角色的防御力，安全区域会加分。
            // 如果过于靠近友方角色，会导致减分（AI不要扎堆）
            // 计算敌人的平均位置，作为【敌人中心点】。
            // 如果移动范围内有障碍物，则障碍物与【敌人中心的】相背的地格能加分。
        }
    }
}