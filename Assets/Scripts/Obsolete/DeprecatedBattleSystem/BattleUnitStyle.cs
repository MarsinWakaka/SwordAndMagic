using DeprecatedBattleSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class BattleUnitStyle : MonoBehaviour
    {
        // 通过调整RectTransform width来调整血条长度
        [SerializeField] private RectTransform healthBar;
        [SerializeField] private RectTransform shieldBar;

        public void UpdateUI(BattleUnit unit)
        {
            healthBar.sizeDelta = new Vector2(unit.health / 2f,1);
            shieldBar.sizeDelta = new Vector2(unit.curDefense / 2f, 1);
        } 
    }
}