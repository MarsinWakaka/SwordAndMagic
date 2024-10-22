using System;
using System.Collections.Generic;
using System.Linq;
using Obsolete;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace DeprecatedBattleSystem
{
    enum RangeType
    {
        MoveRange,
        AttackRange
    }

    public enum ActionButtonType
    {
        None,
        Move,
        Attack
    }

    public class BattleManager : MonoBehaviour
    {
        public CellManager cellManager;
        private BattleUIManager _battleUIManager;
        private MouseStyleHelper _mouseStyle;

        // 地形数据
        private Cell[,] _cells;
        
        [FormerlySerializedAs("selectedFightUnit")] public BattleUnit selectedBattleUnit;
        [FormerlySerializedAs("curFightUnit")] public BattleUnit curBattleUnit;

        public List<BattleUnit> fightUnits;
        private int _curFightUnitIndex;

        // 辅助变量
        List<GameObject> _gosInRange;
        bool[,] _isVisited;
        
        private ActionButtonType _btnSelected = ActionButtonType.None;

        void Start()
        {
            InitComponent();
            RefreshGridData();
            GenerateFightUnit();
            
            // 刷新UI
            RefreshActionBar();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RefreshGridData();
                RefreshFightUnitOccupy();
                RefreshMoveRange();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (curBattleUnit == null) Debug.Log("No curFightUnit");

                curBattleUnit.OnTurnEnter();
                RefreshMoveRange(curBattleUnit.transform.position, curBattleUnit.curMoveRange);
            }
        }

        private void InitComponent()
        {
            cellManager = FindFirstObjectByType<CellManager>();
            cellManager.battleManager = this;
            _battleUIManager = FindFirstObjectByType<BattleUIManager>();
            _mouseStyle = FindFirstObjectByType<MouseStyleHelper>();
            _gosInRange = new List<GameObject>();
        }

        private void RefreshGridData()
        {
            _gosInRange.Clear();
            _cells = cellManager.RegenerateGrid();
            
            foreach (var cell in _cells)
            {
                // cell.GetComponent<MonoBehaviour>().OnMouseDonw += () =>
                // {
                //     if (!cell.cellData.isMoveable) return;
                //     if (curBattleUnit == null) return;
                //     if (cell.moveableGo.activeSelf)
                //     {
                //         TryApplyMove(cell.transform.position);
                //     }
                // };
            }
            
            var size = cellManager.GetGridSize();
            _isVisited = new bool[size.Item1, size.Item2];
        }

        public void GenerateFightUnit()
        {
            // 1、找到所有的FightUnit
            fightUnits = FindObjectsOfType<BattleUnit>().ToList();
            
            // 2、角色按照最大移动范围排序
            fightUnits.Sort((a, b) => b.maxMoveRange.CompareTo(a.maxMoveRange));

            RefreshFightUnitOccupy();

            // 3、初始化参数
            _curFightUnitIndex = 0;
            curBattleUnit = fightUnits[_curFightUnitIndex];
            curBattleUnit.OnTurnEnter();
            _battleUIManager.RefreshCurCharacterInfo(curBattleUnit);
            _btnSelected = ActionButtonType.Move; // 此乃默认状态
        }

        private void RefreshFightUnitOccupy()
        {
            // 将其位置的格子设置为占用
            foreach (var fightUnit in fightUnits)
            {
                var position = fightUnit.transform.position;
                _cells[(int)position.y, (int)position.x].standedGo = fightUnit.gameObject;
            }
        }

        private void EndTurn()
        {
            // 角色索引
            BattleIndexIncrInLoop(1);
            
            curBattleUnit.OnTurnExit();
            curBattleUnit = fightUnits[_curFightUnitIndex];
            curBattleUnit.OnTurnEnter();
            // 此乃默认状态
            _btnSelected = ActionButtonType.Move;

            _battleUIManager.RefreshCurCharacterInfo(curBattleUnit);
            RefreshActionBar();
        }

        #region 按钮点击

        public void BtnClickedMove()
        {
            // 如果是取消点击的按钮
            if(_btnSelected == ActionButtonType.Move)
            {
                CancelBtnSelected();
                return;
            }
            
            _btnSelected = ActionButtonType.Move;
            _battleUIManager.SetButtonSelected(_btnSelected);
            
            RefreshMoveRange();
        }

        public void BtnClickedAttack()
        {
            // 如果是取消点击的按钮
            if(_btnSelected == ActionButtonType.Attack)
            {
                CancelBtnSelected();
                return;
            }
            
            _btnSelected = ActionButtonType.Attack;
            _battleUIManager.SetButtonSelected(_btnSelected);
            
            RefreshAttackRange();
        }
        
        public void BtnClickedEndTurn()
        {
            _btnSelected = ActionButtonType.None;
            _battleUIManager.SetButtonSelected(_btnSelected);
            EndTurn();
        }
        
        public void BtnCloseDetailPanel()
        {
            _battleUIManager.CloseDetailPanel();
        }
        
        private void CancelBtnSelected()
        {
            _btnSelected = ActionButtonType.None;
            _battleUIManager.SetButtonSelected(_btnSelected);
            CloseRange();
        }

        #endregion
        
        #region 范围显示相关

        /// <summary>
        /// 默认显示当前行动单位的攻击范围
        /// </summary>
        public void RefreshAttackRange()
        {
            // 关闭之前的显示
            CloseRange();

            ClearRangeData();

            // BFS显示哪些地图可见
            ShowRange(RangeType.AttackRange);

            OpenRange();
        }

        public void RefreshMoveRange()
        {
            // 关闭之前的显示
            CloseRange();

            ClearRangeData();

            // BFS显示哪些地图可见
            ShowRange(RangeType.MoveRange);

            OpenRange();
        }

        public void RefreshMoveRange(Vector2 startPosition, int range)
        {
            // 关闭之前的显示
            CloseRange();

            ClearRangeData();

            // BFS显示哪些地图可见
            ShowRange(startPosition, range, RangeType.MoveRange);

            OpenRange();
        }

        /// <summary>
        /// 关闭范围显示
        /// </summary>
        private void CloseRange()
        {
            foreach (var go in _gosInRange)
            {
                go.SetActive(false);
            }
        }

        private void ClearRangeData()
        {
            _gosInRange.Clear();
            // 将之前的访问记录清空
            for (int i = 0; i < _isVisited.GetLength(0); i++)
            for (int j = 0; j < _isVisited.GetLength(1); j++)
                _isVisited[i, j] = false;
        }

        private void OpenRange()
        {
            // 显示效果
            foreach (var sgo in _gosInRange)
            {
                sgo.SetActive(true);
            }
        }

        private void ShowRange(RangeType type)
        {
            switch (type)
            {
                case RangeType.MoveRange:
                    MoveRangeBfs(curBattleUnit.transform.position, curBattleUnit.curMoveRange);
                    break;
                case RangeType.AttackRange:
                    AttackRangeRayTracing(curBattleUnit.transform.position, curBattleUnit.attackRange);
                    break;
            }
        }
        
        private void ShowRange(Vector2 startPosition, int range, RangeType type)
        {
            switch (type)
            {
                case RangeType.MoveRange:
                    MoveRangeBfs(startPosition, range);
                    break;
                case RangeType.AttackRange:
                    AttackRangeRayTracing(startPosition, range);
                    break;
            }
        }

        // 显示移动范围
        private void MoveRangeBfs(Vector2 pos, int range)
        {
            // 计算所需
            Queue<Vector2> queue = new Queue<Vector2>();
            int[,] dir = { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };
            
            // 定义当前消耗
            int curStepCost = 0;

            queue.Enqueue(pos);
            SetVisited(pos);
            while (queue.Count > 0 && ++curStepCost <= range)
            {
                int queueSize = queue.Count;                                            // 该轮循环的队列大小
                for (var cnt = 0; cnt < queueSize; cnt++)                               // 遍历该层的所有节点，往四周扩散一格
                {
                    var curPos = queue.Dequeue();
                    for (int index = 0; index < 4; index++)
                    {
                        var nextPos = curPos + new Vector2(dir[index, 0], dir[index, 1]);
                                                                                                    // 剔除不合法的位置
                        if (OutOfBorder(nextPos)) continue;                                         // 1、超出边界
                        if (HasVisited(nextPos)) continue;                                          // 2、已经访问过
                        var cell = GetCellOnPos(nextPos);
                        if (cell.standedGo != null) continue;                                    // 3、有人站着

                        if (cell.cellData.isMoveable)
                        {
                            // 算法必要步骤
                            queue.Enqueue(nextPos);
                            SetVisited(nextPos);
                            // 其它操作 Start
                            cell.cost = curStepCost;
                            _gosInRange.Add(cell.moveableGo);
                            // 其它操作 End
                        }
                    }
                }
            }
        }

        #region 攻击范围计算相关

        private void AttackRangeRayTracing(Vector2 startPos, int range)
        {
            if (range == 0) return;

            int left = (int)startPos.x - range;
            int right = (int)startPos.x + range;
            int top = (int)startPos.y + range;
            int bottom = (int)startPos.y - range;

            SetVisible(startPos);

            for (int x = left; x <= right; x++)
            {
                TraceLine(startPos, new Vector2(x, top), range);
                TraceLine(startPos, new Vector2(x, bottom), range);
            }

            for (int y = bottom; y <= top; y++)
            {
                TraceLine(startPos, new Vector2(left, y), range);
                TraceLine(startPos, new Vector2(right, y), range);
            }
        }

        /// 模拟从视点发出的一条射线，直到碰到障碍物
        private void TraceLine(Vector2 startPos, Vector2 endPos, int range)
        {
            int startX = (int)startPos.x, startY = (int)startPos.y;
            int endX = (int)endPos.x, endY = (int)endPos.y;
            int deltaX = endX - startX;
            int deltaY = endY - startY;
            int signX = deltaX >= 0 ? 1 : -1;
            int signY = deltaY >= 0 ? 1 : -1;
            // 所以乘以一个deltaX也不影响
            deltaX = Math.Abs(deltaX);
            deltaY = Math.Abs(deltaY);

            int baseX = startX;
            int baseY = startY;

            if (deltaX >= deltaY)
            {
                int error = (deltaY << 1) - deltaX;
                int errorInc1 = deltaY << 1;
                int errorInc2 = (deltaY - deltaX) << 1;

                while (baseX != endX)
                {
                    baseX += signX;
                    if (error >= 0)
                    {
                        error += errorInc2;
                        baseY += signY;
                    }
                    else
                    {
                        error += errorInc1;
                    }

                    if (OutOfBorder(baseX, baseY)) break;
                    if (CalcDist(baseX, baseY, startX, startY) > range) break;
                    SetVisible(new Vector2(baseX, baseY));
                    if (BlockLight(baseX, baseY)) break;
                }
            }
            else
            {
                int error = (deltaX << 1) - deltaY;
                int errorInc1 = deltaX << 1;
                int errorInc2 = (deltaX - deltaY) << 1;

                while (baseY != endY)
                {
                    baseY += signY;
                    if (error >= 0)
                    {
                        error += errorInc2;
                        baseX += signX;
                    }
                    else
                    {
                        error += errorInc1;
                    }

                    if (OutOfBorder(baseX, baseY)) break;
                    if (CalcDist(baseX, baseY, startX, startY) > range) break;
                    SetVisible(new Vector2(baseX, baseY));
                    if (BlockLight(baseX, baseY)) break;
                }
            }
        }

        private bool OutOfBorder(int posX, int posY)
        {
            return posX < 0 || posY < 0 || posX >= _cells.GetLength(1) || posY >= _cells.GetLength(0);
        }

        private bool OutOfBorder(Vector2 pos)
        {
            return pos.x < 0 || pos.y < 0 || pos.x >= _cells.GetLength(1) || pos.y >= _cells.GetLength(0);
        }

        private bool BlockLight(int baseX, int baseY)
        {
            return _cells[baseY, baseX].cellData.canObstacleAttack;
        }

        private void SetVisible(Vector2 pos)
        {
            var cellAgo = _cells[(int)pos.y, (int)pos.x].attackableGo;
            if (!_gosInRange.Contains(cellAgo)) _gosInRange.Add(cellAgo);
        }

        #endregion

        #endregion

        #region 角色交互

        public void NotifyUnitClicked(BattleUnit unitClicked)
        {
            if (curBattleUnit == unitClicked) return;
            selectedBattleUnit = unitClicked;
            _battleUIManager.ShowDetailPanel(selectedBattleUnit);
            
            // 玩家启用了攻击
            if (_btnSelected == ActionButtonType.Attack)
            {
                if (CalcDistanceFromCurUnit(unitClicked.transform.position) > curBattleUnit.attackRange) return;
                TryApplyDamage();
            }
        }
        public void NotifyUnitMouseEnter(BattleUnit unitEnter)
        {
            if (_btnSelected == ActionButtonType.Attack)
            {
                _mouseStyle.SetMouseStyle(ref curBattleUnit.attackIcon);
            }
        }
        public void NotifyUnitMouseExit(BattleUnit battleUnit)
        {
            _mouseStyle.ResetMouseStyle();
        }
        public void TryApplyMove(Vector2 targetPos)
        {
            GetCellOnPos(curBattleUnit.transform.position).standedGo = null;
            
            curBattleUnit.curMoveRange -= GetCellOnPos(targetPos).cost;
            curBattleUnit.Move(targetPos);
            RefreshMoveRange();

            GetCellOnPos(targetPos).standedGo = gameObject;

            // 刷新UI
            _battleUIManager.RefreshCurCharacterInfo(curBattleUnit);
        }
        private void TryApplyDamage()
        {
            if (curBattleUnit == null) return;
            // 1、攻击距离检测
            if (CalcDistanceFromCurUnit(selectedBattleUnit.transform.position) > curBattleUnit.attackRange) return;
            // 2、攻击次数检测
            if (curBattleUnit.curAttackCount <= 0) return;
            // 应用攻击效果
            selectedBattleUnit.TakeDamage(curBattleUnit.GetAttackForce());
            curBattleUnit.curAttackCount--;
            
            // 播放音效
            curBattleUnit.PlayAttackFX();

            // 刷新主角UI和受击UI
            _battleUIManager.RefreshCurCharacterInfo(curBattleUnit);
            _battleUIManager.ShowDetailPanel(selectedBattleUnit);
        }
        public void RegisterDeath(BattleUnit deadUnit)
        {
            Debug.Log($"Fight Info: {deadUnit.gameObject.name} died!");
            
            // 1、如果死去的角色在当前角色的索引之前，那么当前角色的索引需要减一
            if (fightUnits.IndexOf(deadUnit) < fightUnits.IndexOf(curBattleUnit)) BattleIndexIncrInLoop(-1);
            
            // 2、清空该角色相关数据
            fightUnits.Remove(deadUnit); // indexof 该数为-1
            GetCellOnPos(deadUnit.transform.position).standedGo = null;
            
            // 2、通知UI面板信息变更
            // 刷新当前角色的移动范围，因为死亡角色占用的格子已经空了
            RefreshActionBar();
            
            if (curBattleUnit == deadUnit)
            {
                CloseRange();
                curBattleUnit = null;
                Debug.LogWarning("curFightUnit die! EndTurn!");
                EndTurn();
            }
        }


        #endregion

        #region 辅助函数

        private Cell GetCellOnPos(Vector2 pos)
        {
            return _cells[(int)pos.y, (int)pos.x];
        }
        
        private void SetVisited(Vector2 pos)
        {
            _isVisited[(int)pos.y, (int)pos.x] = true;
        }
        
        private bool HasVisited(Vector2 pos)
        {
            return _isVisited[(int)pos.y, (int)pos.x];
        }

        private void BattleIndexIncrInLoop(int num)
        {
            int len = fightUnits.Count;
            _curFightUnitIndex = (_curFightUnitIndex + num + len) % len;
            // Debug.Log("curFightUnitIndex: " + _curFightUnitIndex);
        }

        private int CalcDist(int posAx, int posAy, int posBx, int posBy)
        {
            return Math.Abs(posAx - posBx) + Math.Abs(posAy - posBy);
        }

        public int CalcDistanceFromCurUnit(Vector2 startPos)
        {
            var selectedUnitTrans = curBattleUnit.transform.position;
            return Mathf.Abs((int)(startPos.x - selectedUnitTrans.x)) +
                   Mathf.Abs((int)(startPos.y - selectedUnitTrans.y));
        }

        #endregion
        
        #region UI交互封装

        private void RefreshActionBar()
        {
            _battleUIManager.RefreshActionBar(ref fightUnits, _curFightUnitIndex);
        }
        
        #endregion

    }
}