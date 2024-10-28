using BattleSystem.SkillSystem;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class SkillChosenUI : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private UnityEngine.UI.Text skillInfo;
        [SerializeField] private UnityEngine.UI.Text skillDescription;
        [SerializeField] private Image skillSelectProgress;
        [SerializeField] private Button skillReleaseButton;

        private int _maxCount = 1;
        
        private void Awake()
        {
            skillReleaseButton.onClick.AddListener(ReleaseSkillButtonClicked);
        }

        public void SetSkillChosenUI(BaseSkill skill)
        {
            skillIcon.sprite = skill.skillIcon;
            skillInfo.text = GetSKillInfo(skill);
            skillDescription.text = skill.GetDescription();
            _maxCount = skill.maxTargetCount;
            skillSelectProgress.fillAmount = 0;
            EventCenter<GameEvent>.Instance.RemoveListener<int>(
                GameEvent.OnSkillTargetChoseCountChanged, TargetSelectedCountChanged);
            EventCenter<GameEvent>.Instance.AddListener<int>(
                GameEvent.OnSkillTargetChoseCountChanged, TargetSelectedCountChanged);
        }

        private string GetSKillInfo(BaseSkill skill)
        {
            return $"【技能】\t: {skill.skillName}\n【AP消耗】\t: {skill.AP_Cost}AP\n【SP消耗】\t: {skill.SP_Cost}SP\n【冷却】\t: {skill.coolDown}回合";
        }
        
        private void TargetSelectedCountChanged(int count)
        {
            skillSelectProgress.fillAmount = (float)count / _maxCount;
        }
        
        private void ReleaseSkillButtonClicked()
        {
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.OnSkillReleaseButtonClicked);
        }
    }
}