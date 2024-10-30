using ConsoleSystem;
using Entity;
using Entity.Unit;
using UnityEngine;

namespace GamePlaySystem.SkillSystem
{
    [CreateAssetMenu(menuName = "技能/指向性伤害技能", fileName = "指向性伤害技能")]
    public class DirectedDamageSkill : BaseSkill
    {
        [Header("伤害技能特有属性")]
        public DamageType damageType;
        public int damage;

        /// <summary>
        /// 未来会在障碍物情况下进行重写
        /// </summary>
        public override bool isTargetInATKRange(Vector2 casterPosition, Vector2 targetPosition)
        {
            // 使用曼哈顿距离
            return range >= Mathf.Abs(casterPosition.x - targetPosition.x) + Mathf.Abs(casterPosition.y - targetPosition.y);
        }

        public override Vector2[] GetImpactScope(Vector2 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2[] GetReleaseScope(Vector2 startPos)
        {
            var scope = new Vector2[1];
            return scope;
        }

        public override void Execute(Character caster, BaseEntity[] targets)
        {
            foreach (var target in targets)
            {
                if (target is Character character)
                {
                    MyConsole.Print($"{caster.characterName} 对 {character.characterName} 使用了 {skillName}", MessageColor.Red);
                    MyConsole.Print($"\t{character.characterName}被命中，受到了 {damage} 点火焰伤害", MessageColor.Red);
                    character.TakeDamage(damage, damageType);
                }
                else if (target is Tile tile)
                {
                    MyConsole.Print($"{caster.characterName} 对 {tile.entityType} 使用了 {skillName}", MessageColor.Red);
                }
                // TODO 其他类型的实体
            }
        }
    }
}