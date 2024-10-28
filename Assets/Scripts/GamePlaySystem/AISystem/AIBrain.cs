using Entity.Unit;
using System.Collections.Generic;
using Entity.Tiles;

namespace GamePlaySystem.AISystem
{
    public class AIBrain
    {
        CharacterManager _characterManager;
        TileManager _tileManager;
        
        // 交由战斗管理器注入
        public AIBrain(CharacterManager characterManager, TileManager tileManager)
        {
            
        }
        
        private Character _curChar;
        private CharacterProperty _prop;
        // 双向队列
        private List<ICommand> _actions = new();
        
        public void DoTactic(Character character)
        {
            // 初始化
            _curChar = character;
            _prop = _curChar.property;
            _actions.Clear();
            // TODO AI逻辑
            
            // 1、获取战场地图格子
            // 2、绘制危险区域
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
            // 计算敌人的危险度【血量 + 攻击力】减分 // 也可通过固定值代替威胁度
            // 每个敌人都会对周围区域造成威胁，施加的减分随着距离的增加而衰减
            // 每个友方角色的一定范围内都会有一个安全区域，安全区域的大小取决于角色的防御力，安全区域会加分。
            // 如果过于靠近友方角色，会导致减分（AI不要扎堆）
            // 计算敌人的平均位置，作为【敌人中心点】。
            // 如果移动范围内有障碍物，则障碍物与【敌人中心的】相背的地格能加分。
        }
    }
}