using System.Collections.Generic;
using ConsoleSystem;
using Entity;
using Entity.Unit;
using GamePlaySystem.SkillSystem;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;

namespace GamePlaySystem.Controller.AI
{
    public interface ICommand
    {
        public bool Execute();
    }
    
    public class FollowPathCommand : ICommand
    {
        private Character _character;
        private PathNode _destNode;
        
        /// <summary>
        /// 根据终点Node，往前遍历，直到找到起点Node(起点Node.FromNode == null)，从而生成移动路线。
        /// </summary>
        public void Init(Character character, PathNode destNode)
        {
            MyConsole.Print($"【移动路线命令】：{character}将要从{character.transform.position}去到{destNode}",
                MessageColor.Orange);
            this._character = character;
            this._destNode = destNode;
        }
        
        public bool Execute()
        {
            // 移动到目标地点
            _character.transform.position = new Vector3(_destNode.PosX, _destNode.PosY, 0);
            MyConsole.Print($"【移动路线命令】：{_character}移动到了{_destNode.PosX},{_destNode.PosY}", MessageColor.Orange);
            return true;
        }
    }

    public class BaseSkillCommand : ICommand
    {
        private BaseSkill _chosenSkill;
        private Character _caster;
        private BaseEntity[] _targets;
        public void Init(BaseSkill chosenSKill, Character caster, BaseEntity[] targets)
        {
            this._chosenSkill = chosenSKill;
            this._caster = caster;
            this._targets = targets;
            foreach (var position in targets)
            {
                MyConsole.Print($"【使用技能命令】：{caster} 瞄准 {position} 位置使用了 {chosenSKill.skillName}", MessageColor.Orange);
            }
        }

        public bool Execute()
        {
            // 使用技能
            // 通过TileManager获取Tile上的实体
            _chosenSkill.Execute(_caster, _targets);
            return true;
        }
    }
}