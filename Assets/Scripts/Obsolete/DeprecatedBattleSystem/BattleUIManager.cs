using System.Collections;
using System.Collections.Generic;
using BattleSystem;
using UnityEngine;
using UnityEngine.UI;

namespace DeprecatedBattleSystem
{
    public class BattleUIManager : MonoBehaviour
    {
        #region 回合角色信息显示

        [Header("回合中角色信息显示")]
        [SerializeField] private Image characterIcon;
        [SerializeField] private UnityEngine.UI.Text characterName;
        [SerializeField] private UnityEngine.UI.Text characterHealth;
        [SerializeField] private UnityEngine.UI.Text characterDefense;
        [SerializeField] private UnityEngine.UI.Text characterAttackRange;
        [SerializeField] private UnityEngine.UI.Text characterAttack;
        [SerializeField] private UnityEngine.UI.Text characterAttackCountRemain;
        [SerializeField] private UnityEngine.UI.Text characterMoveRange;

        #endregion

        #region 行动面板

        [Header("行动面板")]
        [SerializeField] private Button moveButton;
        [SerializeField] private Button attackButton;
        // [SerializeField] private Button endTurnButton;
        [Tooltip("按钮选中时，与之颜色相乘")]
        [SerializeField] private Color selectedColorMul;
        private ActionButtonType _curBtnSelected = ActionButtonType.None;
        private Color _moveButtonColor;
        private Color _attackButtonColor;

        #endregion

        #region 角色信息查看面板

        [Header("角色信息查看面板")]
        [SerializeField] private GameObject characterDetailPanel;
        [SerializeField] private Image characterDetailIcon;
        [SerializeField] private UnityEngine.UI.Text characterDetailInfo;
        [SerializeField] private UnityEngine.UI.Text characterDetailName;
        [SerializeField] private UnityEngine.UI.Text characterDetailHealth;
        [SerializeField] private UnityEngine.UI.Text characterDetailDefense;
        [SerializeField] private UnityEngine.UI.Text characterDetailAttackRange;
        [SerializeField] private UnityEngine.UI.Text characterDetailAttack;
        [SerializeField] private UnityEngine.UI.Text characterDetailMaxAttackCount;
        [SerializeField] private UnityEngine.UI.Text characterDetailMaxMoveRange;
    

        #endregion
    
        #region 角色攻击顺序面板

        [Header("角色轮询面板")]
        [SerializeField] private RectTransform selectedFramework;
        [SerializeField] private List<Image> turnPreviewBgList;
        [SerializeField] private List<Image> turnPreviewIconList;
        [SerializeField] Color unknownColor;
        [SerializeField] Color playerColor;
        [SerializeField] Color enemyColor;

        #endregion

        private int _previewLen;

        private void Awake()
        {
            _moveButtonColor = moveButton.GetComponent<Image>().color;
            _attackButtonColor = attackButton.GetComponent<Image>().color;
            _previewLen = turnPreviewIconList.Count;

            StartCoroutine(nameof(SelectedFrameworkFX));
        }

        public void RefreshCurCharacterInfo(BattleUnit battleUnit)
        {
            // 刷新角色信息
            characterIcon.sprite = battleUnit.sprite;
            characterName.text = battleUnit.name;
            characterHealth.text = battleUnit.health.ToString();
            characterDefense.text = battleUnit.curDefense.ToString();
            characterAttackRange.text = battleUnit.attackRange.ToString();
            characterAttack.text = battleUnit.minAttack + "~" + battleUnit.maxAttack;
            characterAttackCountRemain.text = battleUnit.curAttackCount.ToString();
            characterMoveRange.text = battleUnit.curMoveRange.ToString();
        }

        #region 详细信息查看

        public void CloseDetailPanel()
        {
            characterDetailPanel.SetActive(false);
        }
    
        public void ShowDetailPanel(BattleUnit battleUnit)
        {
            if(!characterDetailPanel.activeSelf) characterDetailPanel.SetActive(true);
            RefreshCharacterDetailInfo(battleUnit);
        }
    
        public void RefreshCharacterDetailInfo(BattleUnit battleUnit)
        {
            // 刷新角色信息
            characterDetailIcon.sprite = battleUnit.sprite;
            characterDetailName.text = battleUnit.name;
            characterDetailInfo.text = battleUnit.info;
            characterDetailHealth.text = battleUnit.health.ToString();
            characterDetailDefense.text = battleUnit.curDefense.ToString();
            characterDetailAttackRange.text = battleUnit.attackRange.ToString();
            characterDetailAttack.text = battleUnit.minAttack + "~" + battleUnit.maxAttack;
            characterDetailMaxAttackCount.text = battleUnit.maxAttackCount.ToString();
            characterDetailMaxMoveRange.text = battleUnit.maxMoveRange.ToString();
        }
        
        public void SetButtonSelected(ActionButtonType btnClicked)
        {
            // 前后点击是否相同
            if (_curBtnSelected == btnClicked || btnClicked == ActionButtonType.None)
            {
                ClearBtnSelected();
            }
            else
            {
                // 按钮颜色不一样
                switch (btnClicked)
                {
                    case ActionButtonType.Attack:
                        moveButton.image.color = _moveButtonColor;
                        attackButton.image.color = _attackButtonColor * selectedColorMul;
                        break;
                    case ActionButtonType.Move:
                        attackButton.image.color = _attackButtonColor;
                        moveButton.image.color = _moveButtonColor * selectedColorMul;
                        break;
                }
            
                _curBtnSelected = btnClicked;
            }
        }

        private void ClearBtnSelected()
        {
            // 颜色归位
            switch (_curBtnSelected)
            {
                case ActionButtonType.Attack:
                    attackButton.image.color = _attackButtonColor;
                    break;
                case ActionButtonType.Move:
                    moveButton.image.color = _moveButtonColor;
                    break;
            }
            // cancel selected
            _curBtnSelected = ActionButtonType.None;
            // Debug.Log("Cancel selected");
        }

        #endregion

        #region 角色轮询面板相关函数

        public void RefreshActionBar(ref List<BattleUnit> characters, int curIndex)
        {
            int charLen = characters.Count;
            for (int i = 0; i < _previewLen; i++)
            {
                var unit = characters[(curIndex + i) % charLen];
                var type = unit.racialType;
            
                switch (type)
                {
                    case RacialType.Player:
                        turnPreviewBgList[i].color = playerColor;
                        break;
                    case RacialType.Enemy:
                        turnPreviewBgList[i].color = enemyColor;
                        break;
                    default:
                        turnPreviewBgList[i].color = unknownColor;
                        break;
                }
            
                turnPreviewIconList[i].sprite = unit.sprite;
            }
        }

        private IEnumerator SelectedFrameworkFX()
        {
            while (true)
            {
                selectedFramework.localScale = Vector3.one * (Mathf.Sin(Time.time * 5) * 0.03f + 1);
                yield return null;
            }
        }

        #endregion
    }
}