using System.Collections.Generic;
using BattleSystem.FactionSystem;
using Entity.Unit;
using GamePlaySystem.AISystem;
using GamePlaySystem.Controller.AI.AIDecisionResource;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public class AIBrain
    {
        private CharacterManager _characterManager;
        private TileManager _tileManager;
        private NavigationProvider _navigationProvider;

        private readonly RiskHeatMapCooker _riskHeatMapCooker;
        
        // 交由战斗管理器注入
        public AIBrain(CharacterManager characterManager, TileManager tileManager, NavigationProvider navigationProvider)
        {
            _characterManager = characterManager;
            _tileManager = tileManager;
            _navigationProvider = navigationProvider;
            _riskHeatMapCooker = new RiskHeatMapCooker(_characterManager, _tileManager);
        }
        
        private const float ScoreEnoughToAct = 100f;
        private const float KillHostileScore = 50f;
        private const float KillFriendlyScore = -50f;
        
        private Character _decisionMaker;
        private CharacterProperty _prop;
        private FactionType _hostileFaction;
        private List<Character> _hostiles;
        
        // 双向队列
        private readonly List<ICommand> _actions = new();
        private float _maxScore = 0;
        private List<SkillSlot> skillSlots = new();
        
        /// <summary>
        /// 尽量返回当前决策层下的最高得分的决策
        /// </summary>
        private float DoTactic(float lastScore, int remainActionCount, int rwr, int ap, int sp, Character character)
        {
            float thisLayerScore = lastScore;
            if (remainActionCount <= 0) return lastScore;
            if (ap <= 0 && sp <= 0) return lastScore;
            foreach (var slot in skillSlots)
            {
                var skill = slot.skill;
                if (slot.RemainCoolDown.Value > 0) continue;
                if (skill.AP_Cost > ap || skill.SP_Cost > sp) continue;
                var range = skill.range;
                var maxReachDist = range + rwr;
                // 如果是伤害技能
                if (skill is DirectedDamageSkill damageSkill)
                {
                    var damage = damageSkill.damage;
                    switch (damageSkill.impactType)
                    {
                        case ImpactType.Single:
                            foreach (var hostile in _hostiles)
                            {
                                if (damageSkill.canImpactFactionType.HasFlag(hostile.Faction.Value))
                                {
                                    // 技能能否命中敌人
                                    if (damageSkill.isTargetInATKRange(
                                            character.transform.position, 
                                            hostile.transform.position))
                                    {
                                        // 生成动作命令，考虑对象池
                                        var singleDirectSKill = new SingleDirectSKill();
                                        singleDirectSKill.Init(damageSkill, character, hostile.transform.position);
                                        _actions.Add(singleDirectSKill);
                                        // 计算消耗 并 递归
                                        thisLayerScore = Mathf.Max(_maxScore, 
                                            DoTactic(lastScore + GetAttackScore(damageSkill, hostile), 
                                                remainActionCount - 1, 
                                                rwr, ap - skill.AP_Cost, sp - skill.SP_Cost, character));
                                        if (thisLayerScore < _maxScore) {
                                            _actions.RemoveAt(_actions.Count - 1); //此层得分不不如其它同层的得分，移除此动作
                                        }
                                    }
                                }
                            }
                            break;
                        case ImpactType.Aoe: // TODO 未完成 // 退一步，AOE技能只会瞄准敌人来减少遍历，不再对范围内所有格子释放(即使有可能得到更高的得分)
                            break;
                    }
                }
            }
            if (rwr > 0)
            {
                // 根据危险热力图决定分数
                var pathNodes = _navigationProvider.GetReachablePositions(
                    (int)character.transform.position.x, 
                    (int)character.transform.position.y, 
                    rwr);
                foreach (var pathNode in pathNodes)
                {
                    // 生成动作命令，考虑对象池
                    var moveCommand = new MoveCommand();
                    moveCommand.Init(character, new Vector2(pathNode.PosX, pathNode.PosY));
                    _actions.Add(moveCommand);
                    // 计算消耗 并 递归
                    thisLayerScore = Mathf.Max(_maxScore, 
                        DoTactic(0, remainActionCount - 1, 
                            rwr - pathNode.Cost, ap, sp, character));
                    if (thisLayerScore < _maxScore) {
                        _actions.RemoveAt(_actions.Count - 1); //此层得分不不如其它同层的得分，移除此动作
                    }
                }
            }
            return thisLayerScore;
        }
        
        private float GetAttackScore(DirectedDamageSkill skill, Character target)
        {
            var damage = skill.damage;
            var score = (float)damage;
            if (target.property.HP.Value <= damage) {
                score += KillHostileScore;
            }
            return score;
        }
        
        public void DoTactic(Character decisionMaker)
        {
            _actions.Clear();
            // 初始化
            _decisionMaker = decisionMaker;
            _prop = _decisionMaker.property;
            skillSlots = decisionMaker.Skills;
            _hostileFaction = (1 - decisionMaker.Faction.Value); // TODO 只当仅有两个阵营此代码有效
            _hostiles = _characterManager.GetUnitsByFaction(_hostileFaction);
            _maxScore = 0;
            // TODO AI逻辑
            
            // 1、获取战场地图格子
            var tiles = _tileManager.GetTiles();
            // 2、绘制危险区域
            var riskMap = _riskHeatMapCooker.Cook(_decisionMaker.Faction.Value);
            // 3、递归策略函数得到最佳决策结果
            // 4、执行决策
            
            // 角色行动资源【RWR，AP，SP】
            // 【策略函数 - HP状态正常】
            // Dequeue<Command> actions (用于描述角色的行动，注意池化)
            // int DoTactic(Character char, int curScore)
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