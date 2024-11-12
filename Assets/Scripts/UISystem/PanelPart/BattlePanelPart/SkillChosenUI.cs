using GamePlaySystem.SkillSystem;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class SkillChosenUI : MonoBehaviour
    {
        [Header("ICON FromAtlas")]
        [SerializeField] private Image iconAP;
        [SerializeField] private Image iconSP;
        [SerializeField] private Image iconCoolDownTime;
        [SerializeField] private Image iconRange;
        [SerializeField] private Image skillChosenUI;
        [Header("技能信息")]
        [SerializeField] private Image skillIcon;
        [SerializeField] private Text skillName;
        [SerializeField] private Text apCost;
        [SerializeField] private Text spCost;
        [SerializeField] private Text range;
        [SerializeField] private Text coolDown;
        [SerializeField] private Text skillDescription;
        [Header("技能状态")]
        [SerializeField] private Image skillSelectProgress;
        [SerializeField] private Button skillReleaseButton;
        
        private int _maxCount = 1;
        
        private void Awake()
        {
            skillReleaseButton.onClick.AddListener(ReleaseSkillButtonClicked);
        }
        public void SetSpriteResources(Sprite iconAP, Sprite iconSP, Sprite iconCoolDownTime, Sprite iconRange, Sprite skillChosenSprite)
        {
            this.iconAP.sprite = iconAP;
            this.iconSP.sprite = iconSP;
            this.iconCoolDownTime.sprite = iconCoolDownTime;
            this.iconRange.sprite = iconRange;
        }

        public void SetSkillChosenUI(BaseSkill skill)
        {
            skillIcon.sprite = skill.skillIcon;
            skillName.text = skill.skillName;
            apCost.text = $"{skill.AP_Cost}AP";
            spCost.text = $"{skill.SP_Cost}SP";
            range.text = $"{skill.range}m";
            coolDown.text = $"{skill.coolDown}回合";
            skillDescription.text = skill.GetDescription();
            _maxCount = skill.maxTargetCount;
            skillSelectProgress.fillAmount = 0;
            EventCenter<GameEvent>.Instance.RemoveListener<int>(
                GameEvent.OnSkillTargetChoseCountChanged, TargetSelectedCountChanged);
            EventCenter<GameEvent>.Instance.AddListener<int>(
                GameEvent.OnSkillTargetChoseCountChanged, TargetSelectedCountChanged);
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