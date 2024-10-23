using UnityEngine;

namespace BattleSystem.SkillSystem
{
    public class SkillsLoader : MonoBehaviour
    {
        // 可以由其他类加载
        private void Awake()
        {
            LoadSkills();
        }

        private void LoadSkills()
        {
            SkillRegistry.RegisterSkill(new DirectedDamageSkill());
        }
    }
}