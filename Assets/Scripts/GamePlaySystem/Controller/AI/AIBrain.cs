using System.Collections.Generic;
using ConsoleSystem;
using Entity;
using GamePlaySystem.ControlCommand;
using GamePlaySystem.Controller.AI.AIDecisionResource;
using GamePlaySystem.FactionSystem;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using GamePlaySystem.TileSystem.ViewField;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public class AIBrain : IBrain
    {
        private readonly CharacterManager characterManager;
        private readonly TileManager _tileManager;
        private readonly INavigationService navigationService;
        private readonly ScoreHeatMapCooker scoreHeatMapCooker;
        
        // 交由战斗管理器注入
        public AIBrain(CharacterManager characterManager, TileManager tileManager,
            INavigationService navigationService)
        {
            this.characterManager = characterManager;
            _tileManager = tileManager;
            this.navigationService = navigationService;
            scoreHeatMapCooker = new ScoreHeatMapCooker(this.characterManager, _tileManager);
        }

        private const float KillHostileScore = 50f;
        private const float KillFriendlyScore = -50f;   // AOE专属

        private float[,] _riskMap;
        private Character _decider;
        private Transform _deciderTrans;
        private List<SkillSlot> _deciderSkills;
        private FactionType _hostileFaction;
        private List<Character> _hostiles;

        private readonly Queue<ICommand> _actions = new();

        private const MessageColor FontColor = MessageColor.Gray;

        /// <summary>
        /// 尽量返回当前决策层下的最高得分的决策
        /// </summary>
        private void DoTactic(Character decider, int remainActionCnt, int rwr, int ap, int sp, Vector3 virtualPos)
        {
            var maxIterationRestrict = 10;
            // 角色还能做出除了移动以外的动作，则进行攻击尝试
            MakeAttackDecisionStage:
            while (remainActionCnt > 0 && ap > 0 && sp >= 0)
            {
                if (maxIterationRestrict-- <= 0)
                {
                    Debug.LogWarning("【AI】: 迭代策略函数超过最大迭代次数，强制结束");
                    break;
                }
                // 这是攻击阶段的循环，分为攻击判断阶段以及距离不足时的向前靠近阶段
#if UNITY_EDITOR
                MyConsole.Print($"【AI】: {decider.CharacterName} 进入攻击阶段决策判断：", MessageColor.Yellow);
#endif
                SkillSlot skillSlotChosen = null;
                var hasAnyUnitCanImpact = false;
                var farthestSkillRange = 0;
                var maxActionScore = 0f;
                var targetsImpacted = new List<BaseEntity>();
                foreach (var slot in _deciderSkills)
                {
                    var skill = slot.skill;
                    // 技能可释放前提条件检查：冷却时间、行动资源消耗
                    if (slot.RemainCoolDown.Value > 0 || skill.AP_Cost > ap || skill.SP_Cost > sp)
                    {
#if UNITY_EDITOR
                        MyConsole.Print($"【AI】: 技能{skill.skillName}未满足技能释放条件", FontColor);
#endif
                        continue; 
                    }
                    hasAnyUnitCanImpact = false;
                    // 如果是伤害技能
                    if (skill is DirectedDamageSkill damageSkill)
                    {
                        switch (damageSkill.impactType)
                        {
                            case ImpactType.Single:
                                foreach (var hostile in _hostiles)
                                {
                                    if (hostile.IsDead) continue;
                                    if (!FactionManager.CanImpact(_decider, hostile, skill.canImpactFactionType)) continue;
                                    var hostilePos = hostile.transform.position;
                                    hasAnyUnitCanImpact = true;
                                    farthestSkillRange = Mathf.Max(farthestSkillRange, damageSkill.range);
                                    // 技能能否命中敌人
                                    if (damageSkill.isTargetInATKRange( virtualPos,hostilePos))
                                    {
                                        var actionScore = GetAttackScore(damageSkill, hostile);
                                        // 如果得分高于最高得分，则保存此动作
                                        if (actionScore > maxActionScore)
                                        {
#if UNITY_EDITOR
                                            MyConsole.Print($"【AI】: 选择技能{skill.skillName}攻击{hostile.CharacterName}，得分：{actionScore}", FontColor);
#endif
                                            maxActionScore = actionScore;
                                            // 生成动作命令，考虑对象池
                                            skillSlotChosen = slot;
                                            targetsImpacted.Clear();
                                            targetsImpacted.Add(hostile);
                                        }
                                    }
#if UNITY_EDITOR
                                    else
                                    {
                                        MyConsole.Print($"【AI】: 技能{skill.skillName}无法攻击到{hostile.CharacterName}" +
                                                        $"，攻击距离为{skill.range},两者距离{decider.transform.position}->{hostilePos}", FontColor);
                                    }
#endif
                                }
                                break;
                            // 退一步，AOE技能只会瞄准敌人来减少遍历，不再对范围内所有格子释放(即使有可能得到更高的得分)
                            // case ImpactType.Aoe:
                                // break;
                        }
                    }
                }
                // 如果有技能可以释放
                if (skillSlotChosen != null)
                {
#if UNITY_EDITOR
                    MyConsole.Print($"【AI】: 最终决定释放技能{skillSlotChosen.skill.skillName}", MessageColor.Purple);
#endif
                    // TODO 需要重新设置技能的冷却
                    _actions.Enqueue(GetSkillCommand(skillSlotChosen, decider, targetsImpacted.ToArray()));
                    // 计算消耗
                    remainActionCnt--;
                    ap -= skillSlotChosen.skill.AP_Cost;
                    sp -= skillSlotChosen.skill.SP_Cost;
                    skillSlotChosen.RemainCoolDown.Value = skillSlotChosen.skill.coolDown;
                }
                // 如果是因为距离不够导致没有合适的技能释放, 则考虑前进一段距离，再次递归技能，否则寻找最佳位置结束回合。
                else if (hasAnyUnitCanImpact && rwr > 0) {  // 还能移动
#if UNITY_EDITOR
                    MyConsole.Print("【AI】: 因为技能攻击距离不够，进入移动阶段决策判断：", FontColor);
#endif
                    // 如果是因为距离原因导致没有合适的技能释放, 则考虑朝某个角色移动一段距离，再次递归技能，否则寻找最佳位置结束回合。
                    var pathNodesDict = navigationService.GetReachablePositionDict(virtualPos, rwr); 
                    var viewService = ServiceLocator.Get<IViewFieldService>();
                    foreach (var hostile in _hostiles)
                    {
                        var hostilePos = hostile.transform.position;
                        int hostilePosKey = NavigationService.GetIndexKey((int)hostilePos.x, (int)hostilePos.y);
                        // 获取可攻击到敌人的地格
                        var canImpactNodeKeys = viewService.GetViewFieldSets(hostilePos, farthestSkillRange);
                        foreach (var nodeKey in canImpactNodeKeys)
                        {
                            if (nodeKey == hostilePosKey) continue; // 不考虑敌人所在地格
                            if (pathNodesDict.TryGetValue(nodeKey, out var destNode))
                            {
                                var moveDist = Random.Range(1, rwr + 1);
#if UNITY_EDITOR
                                MyConsole.Print($"【AI】: 敌人{hostile.CharacterName}在攻击范围内，前进{moveDist}格子", FontColor);
#endif
                                while (moveDist < destNode.Cost && destNode.FromNode != null) 
                                    destNode = destNode.FromNode;
                                
                                if (Mathf.Approximately(destNode.PosX, virtualPos.x) && 
                                    Mathf.Approximately(destNode.PosY, virtualPos.y)) {
                                    Debug.LogWarning("【AI】: 选择可攻击地点为原地，不应该出现此情况。");
                                    continue;
                                }
                                // 【生成动作命令】
                                var followPathCommand = new FollowPathCommand();
                                followPathCommand.Init(decider, destNode);
                                _actions.Enqueue(followPathCommand);
                                // 【更新参数】
                                rwr -= destNode.Cost;
                                virtualPos = new Vector3(destNode.PosX, destNode.PosY);
                                // 移动后重新进入攻击阶段
                                goto MakeAttackDecisionStage;
                            }
                        }
                    }
                    goto FinalMoveStage;
                }
                else
                {
                    // 没有行动资源或者所有技能都没准备好，结束回合
                    goto FinalMoveStage;
                }
            }
            
            FinalMoveStage:
            // 角色行动阶段，利用剩余行动资源移动至得分最高的地格。
            if (rwr > 0)
            {
#if UNITY_EDITOR
                MyConsole.Print("【AI】: 进入最佳落脚点选择阶段：", FontColor);
#endif
                float maxScore = 0;
                // 获取可到达的地格
                var pathNodesDict = navigationService.GetReachablePositionDict(virtualPos, rwr);
                
                // 根据危险热力图决定分数
                var bestPos = virtualPos;
                // 遍历所有可到达的地格,取得分最高的地格。
                foreach (var pathNode in pathNodesDict)
                {
                    var posX = pathNode.Value.PosX;
                    var posY = pathNode.Value.PosY;
                    var score = _riskMap[posX, posY];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestPos.x = posX;
                        bestPos.y = posY;
                    }
                }
                if (bestPos != virtualPos)
                {
                    // 【生成动作命令】
                    var followPathCommand = new FollowPathCommand();
                    var destNode = pathNodesDict[NavigationService.GetIndexKey((int)bestPos.x, (int)bestPos.y)];
                    followPathCommand.Init(decider, destNode);
                    _actions.Enqueue(followPathCommand);
                    // rwr -= destNode.Cost;
                }
            }
        }
        
        private float GetAttackScore(DirectedDamageSkill skill, Character target)
        {
            var damage = skill.damage;
            var score = (float)damage;
            if (target.Property.HP.Value <= damage)
            {
                score += KillHostileScore;
                // TODO 如果性能允许，则更新危险热力图
            }
            return score;
        }

        private BaseSkillCommand GetSkillCommand(SkillSlot skill, Character caster, BaseEntity[] targets)
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
            _riskMap = scoreHeatMapCooker.CookScoreMap(_decider);
            // 3、递归策略函数得到最佳决策结果
            var prop = _decider.Property;
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