using BattleSystem.Entity.Character;
using Entity;
using Entity.Character;
using MyEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.PanelPart.BattlePanelPart
{
    public class InvestigationPanel : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        [Header("角色名称")]
        [SerializeField] Text characterName;
        [Header("生命值")]
        [SerializeField] RectTransform curHpBar;
        [SerializeField] RectTransform maxHpBar;
        [SerializeField] Text hpText;
        [Header("物理防御")]
        [SerializeField] RectTransform curDefBar;
        [SerializeField] RectTransform maxDefBar;
        [SerializeField] Text defText;
        [Header("魔法防御")]
        [SerializeField] RectTransform curMdefBar;
        [SerializeField] RectTransform maxMdefFBar;
        [SerializeField] Text mdefText;
        
        private Character lastCharacter;
        private CharacterProperty property;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            EventCenter<GameEvent>.Instance.AddListener<BaseEntity>(GameEvent.SetHoverEntity, UpdateInvestigationUI);
        }
        
        private void UpdateInvestigationUI(BaseEntity entity)
        {
            if (entity is Character character) {
                if (character == lastCharacter) return;
                characterName.text = character.characterName;
                SetInvestigationUI(character);
                return;
            }
            CloseInvestigationUI();
        }

        // 考虑到后续会有BUFF列表，所以这里传入的是Character
        private void SetInvestigationUI(Character character)
        {
            canvasGroup.alpha = 1;
            if (character == lastCharacter) return;
            if (lastCharacter != null)
                RemoveListener(lastCharacter.property);
            
            property = character.property;
            AddListener(property);
            RedrawAll(character);
            
            lastCharacter = character;
        }
        
        public void CloseInvestigationUI()
        {
            canvasGroup.alpha = 0;
            if (lastCharacter == null) return;
            RemoveListener(lastCharacter.property);
            lastCharacter = null;
        }
        
        private void AddListener(CharacterProperty prop)
        {
            // TODO 正确做法应该是HP 监听与 HP_MAX 监听分开
            prop.HP.OnValueChanged += RedrawHpBar;
            prop.HP_MAX.OnValueChanged += RedrawMaxHpBar;
            prop.DEF.OnValueChanged += RedrawDefBar;
            prop.DEF_MAX.OnValueChanged += RedrawMaxDefBar;
            prop.MDEF.OnValueChanged += RedrawMdefBar;
            prop.MDEF_MAX.OnValueChanged += RedrawMaxMdefBar;
        }

        private void RemoveListener(CharacterProperty prop)
        {
            prop.HP.OnValueChanged -= RedrawHpBar;
            prop.HP_MAX.OnValueChanged -= RedrawMaxHpBar;
            prop.DEF.OnValueChanged -= RedrawDefBar;
            prop.DEF_MAX.OnValueChanged -= RedrawMaxDefBar;
            prop.MDEF.OnValueChanged -= RedrawMdefBar;
            prop.MDEF_MAX.OnValueChanged -= RedrawMaxMdefBar;
        }
        
        private void RedrawAll(Character character)
        {
            // 此绘制方式只适用于固定大小的UI
            // 设置动态UI
            RedrawHpBar(property.HP.Value);
            RedrawMaxHpBar(property.HP_MAX.Value);
            RedrawDefBar(property.DEF.Value);
            RedrawMaxDefBar(property.DEF_MAX.Value);
            RedrawMdefBar(property.MDEF.Value);
            RedrawMaxMdefBar(property.MDEF_MAX.Value);
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
    }
}