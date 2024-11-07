using Data;
using Entity;
using GamePlaySystem;
using GamePlaySystem.SkillSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class CharacterStatusUI : MonoBehaviour
    {
        #region UI组件
        [Header("UI组件")]
        [Header("角色头像")]
        [SerializeField] private Image charAvatar;
        [SerializeField] private Text charName;
        [Header("生命值")]
        [SerializeField] private RectTransform curHpBar;
        [SerializeField] private RectTransform maxHpBar;
        [SerializeField] private Text hpText;
        [Header("物理防御")]
        [SerializeField] private RectTransform curDefBar;
        [SerializeField] private RectTransform maxDefBar;
        [SerializeField] private Text defText;
        [Header("魔法防御")] 
        [SerializeField] private RectTransform curMdefBar;
        [SerializeField] private RectTransform maxMdefFBar;
        [SerializeField] private Text mdefText;
        [Header("行动力")]
        [SerializeField] private RectTransform actionPointBar;
        [SerializeField] private RectTransform maxActionPointBar;
        [Header("技能点")]
        [SerializeField] private RectTransform skillPointBar;
        [SerializeField] private RectTransform maxSkillPointBar;
        [Header("移动力面板")]
        [SerializeField] private RectTransform remainWalkRangeBar;
        [SerializeField] private RectTransform maxWalkRangeBar;
        [SerializeField] private Text rwrText;
        #endregion
        
        // 当前展示的角色
        private Character lastCharacter;
        private CharacterProperty property;
        
        public void Initialize()
        {
            lastCharacter = null;
            property = null;
        }
        
        public void Uninitialize()
        {
            // 减少对象引用计数
            if (lastCharacter != null) RemoveListener(lastCharacter.Property);
            lastCharacter = null;
            property = null;
        }

        // 基本信息展示，由父级UI监听事件并调用
        public void SetStatusUI(Character character)
        {
            if (character == lastCharacter) return;
            if (lastCharacter != null)
                RemoveListener(lastCharacter.Property);
            
            property = character.Property;
            AddListener(property);
            RedrawAll(character);
            lastCharacter = character;
        }

        /// <summary>
        /// 如果显示的内容在不切换选中角色的情况下也会变的话，则需要对该属性添加监听
        /// (例如HP，玩家选择的角色没变，如果受到伤害，则需要更新UI)
        /// (例如人物头像，只要确保选中角色不变，这个内容就不会更新)
        /// </summary>
        private void AddListener(CharacterProperty prop)
        {
            // TODO 正确做法应该是HP 监听与 HP_MAX 监听分开
            prop.HP.OnValueChanged += RedrawHpBar;
            prop.HP_MAX.OnValueChanged += RedrawMaxHpBar;
            prop.DEF.OnValueChanged += RedrawDefBar;
            prop.DEF_MAX.OnValueChanged += RedrawMaxDefBar;
            prop.MDEF.OnValueChanged += RedrawMdefBar;
            prop.MDEF_MAX.OnValueChanged += RedrawMaxMdefBar;
            prop.AP.OnValueChanged += RedrawAPBar;
            prop.SP.OnValueChanged += RedrawSpBar;
            prop.RWR.OnValueChanged += RedrawRwrBar;
            prop.WR_MAX.OnValueChanged += RedrawMaxWrBar;
        }

        private void RemoveListener(CharacterProperty prop)
        {
            prop.HP.OnValueChanged -= RedrawHpBar;
            prop.HP_MAX.OnValueChanged -= RedrawMaxHpBar;
            prop.DEF.OnValueChanged -= RedrawDefBar;
            prop.DEF_MAX.OnValueChanged -= RedrawMaxDefBar;
            prop.MDEF.OnValueChanged -= RedrawMdefBar;
            prop.MDEF_MAX.OnValueChanged -= RedrawMaxMdefBar;
            prop.AP.OnValueChanged -= RedrawAPBar;
            prop.SP.OnValueChanged -= RedrawSpBar;
            prop.RWR.OnValueChanged -= RedrawRwrBar;
            prop.WR_MAX.OnValueChanged -= RedrawMaxWrBar;
        }
        
        private void RedrawAll(Character character)
        {
            // 设置不动UI
            charAvatar.sprite = character.sprite;
            charName.text = character.CharacterName;
            // 设置动态UI
            RedrawHpBar(property.HP.Value);
            RedrawMaxHpBar(property.HP_MAX.Value);
            RedrawDefBar(property.DEF.Value);
            RedrawMaxDefBar(property.DEF_MAX.Value);
            RedrawMdefBar(property.MDEF.Value);
            RedrawMaxMdefBar(property.MDEF_MAX.Value);
            RedrawAPBar(property.AP.Value);
            RedrawSpBar(property.SP.Value);
            RedrawRwrBar(property.RWR.Value);
            RedrawMaxWrBar(property.WR_MAX.Value);
        }
        
        private void RedrawHpBar(int newHp)
        {
            var hpPercent = (float)newHp / property.HP_MAX.Value;
            curHpBar.sizeDelta = new Vector2(hpPercent * maxHpBar.sizeDelta.x, curHpBar.sizeDelta.y);
            hpText.text = $"{newHp}/{property.HP_MAX.Value}";
        }

        private void RedrawMaxHpBar(int newMaxHp)
        {
            var hpPercent = (float)property.HP.Value / newMaxHp;
            curHpBar.sizeDelta = new Vector2(hpPercent * maxHpBar.sizeDelta.x, curHpBar.sizeDelta.y);
            hpText.text = $"{property.HP.Value}/{newMaxHp}";
        }
        
        private void RedrawDefBar(int newDef)
        {
            float defPercent = property.DEF_MAX.Value == 0 ? 0f : (float)newDef / property.DEF_MAX.Value;
            curDefBar.sizeDelta = new Vector2(defPercent * maxDefBar.sizeDelta.x, curDefBar.sizeDelta.y);
            defText.text = $"{newDef}/{property.DEF_MAX.Value}";
        }
        
        private void RedrawMaxDefBar(int newMaxDef)
        {
            float defPercent = newMaxDef == 0 ? 0f : (float)property.DEF.Value / newMaxDef;
            curDefBar.sizeDelta = new Vector2(defPercent * maxDefBar.sizeDelta.x, curDefBar.sizeDelta.y);
            defText.text = $"{property.DEF.Value}/{newMaxDef}";
        }

        private void RedrawMdefBar(int newMdef)
        {
            float mdefPercent = property.MDEF_MAX.Value == 0 ? 0f : (float)newMdef / property.MDEF_MAX.Value;
            curMdefBar.sizeDelta = new Vector2(mdefPercent * maxMdefFBar.sizeDelta.x, curMdefBar.sizeDelta.y);
            mdefText.text = $"{newMdef}/{property.MDEF_MAX.Value}";
        }
        
        private void RedrawMaxMdefBar(int newMaxMdef)
        {
            float mdefPercent = newMaxMdef == 0 ? 0f : (float)property.MDEF.Value / newMaxMdef;
            curMdefBar.sizeDelta = new Vector2(mdefPercent * maxMdefFBar.sizeDelta.x, curMdefBar.sizeDelta.y);
            mdefText.text = $"{property.MDEF.Value}/{newMaxMdef}";
        }
        
        private void RedrawAPBar(int newAP)
        {
            var apPercent = (float)newAP / CharacterProperty.AP_MAX;
            actionPointBar.sizeDelta = new Vector2(apPercent * maxActionPointBar.sizeDelta.x, actionPointBar.sizeDelta.y);
        }
        
        private void RedrawSpBar(int newSp)
        {
            var spPercent = (float)newSp / CharacterProperty.SP_MAX;
            skillPointBar.sizeDelta = new Vector2(spPercent * maxSkillPointBar.sizeDelta.x, skillPointBar.sizeDelta.y);
        }
  
        private void RedrawRwrBar(int newRwr)
        {
            var rwrPercent = (float)newRwr / property.WR_MAX.Value;
            remainWalkRangeBar.sizeDelta = new Vector2(rwrPercent * maxWalkRangeBar.sizeDelta.x, remainWalkRangeBar.sizeDelta.y);
            rwrText.text = $"{newRwr}/{property.WR_MAX.Value}";
        }
        
        private void RedrawMaxWrBar(int maxWr)
        {
            var rwrPercent = (float)property.RWR.Value / maxWr;
            remainWalkRangeBar.sizeDelta = new Vector2(rwrPercent * maxWalkRangeBar.sizeDelta.x, remainWalkRangeBar.sizeDelta.y);
            rwrText.text = $"{property.RWR.Value}/{maxWr}";
        }
    }
}