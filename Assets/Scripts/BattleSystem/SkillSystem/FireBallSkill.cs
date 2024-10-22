using ConsoleSystem;
using Entity;
using Entity.Character;
using Entity.Tiles;
using UnityEngine;

namespace BattleSystem.SkillSystem
{
    [CreateAssetMenu(menuName = "技能/素能法术/火球术", fileName = "火球术")]
    public class FireBallSkill : BaseSkill
    {
        [Header("火球术特有属性")]
        public int damage;

        public override bool isTargetInRange(Vector2 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2[] GetImpactScope(Vector2 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2[] GetReleaseScope(Vector2 targetPosition)
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
                    character.TakeDamage(damage, DamageType.Magic);
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