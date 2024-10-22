using System;
using DeprecatedBattleSystem;
using UnityEngine;

namespace BattleSystem
{
    public class CellDataEidtor : MonoBehaviour
    {
        CellManager cellManager;
        
        private void Start()
        {
            cellManager = FindObjectOfType<CellManager>();
            cellManager.RegenerateGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 发射2D射线
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null)
                {
                    
                }
            }
        }
        
        public void SaveCellDataBtnClicked()
        {
            var cells = FindObjectsOfType<Cell>();
            // cellManager.SaveCellTypeData(cells);
        }
        
        private void SetCell(Cell cell, int newType)
        {
            // cell.cellData.cellSprite = cellManager.cellSprites[newType];
        }
    }
}