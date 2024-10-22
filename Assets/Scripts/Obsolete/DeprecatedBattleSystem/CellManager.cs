using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

namespace DeprecatedBattleSystem
{
    public class CellManager : MonoBehaviour
    {
        [FormerlySerializedAs("cols")] [SerializeField] int horizontalCount = 10;
        [FormerlySerializedAs("rows")] [SerializeField] int verticalCount = 8;
        
        // 地形列表
        [Tooltip("0: 草地, 1: 石头, 2: 树林, 3: 河流, 4: 桥")]
        private int[,] _cellTypes;
        private Cell[,] _cells;
        [SerializeField] Sprite[] cellSprites;

        public GameObject cellPrefab;
        public BattleManager battleManager;

        [Tooltip("设置Cell的Z值，数值越大，离相机越远")]
        [SerializeField] int cellZ = 1;

        public Cell[,] RegenerateGrid()
        {
            DestroyGrid();
            _cellTypes = GenerateCellTypeData();
            _cells = GenerateGrid(_cellTypes);
            return _cells;
        }
        
        public Tuple<int ,int> GetGridSize()
        {
            return new Tuple<int, int>(verticalCount, horizontalCount);
        }

        private Cell[,] GenerateGrid(int[,] cellTypeData)
        {
            // 注意脚本加载顺序
            Cell[,] cells = new Cell[verticalCount, horizontalCount];
            // cellSprites = Resources.LoadAll<Sprite>("Sprites/Cells");
            
            for (int y = 0; y < verticalCount; y++)
            {
                for(int x = 0; x < horizontalCount; x++)
                {
                    var newCell = Instantiate(cellPrefab, new Vector3(x, y, cellZ), Quaternion.identity);
                    newCell.transform.parent = transform;
                    newCell.name = $"Cell_{y+1}_{x+1}";
                    
                    // 设置单元格属性
                    var cellData = new CellData();
                    int cellType = cellTypeData[y, x];
                    cellData.cellSprite = cellSprites[cellType];
                    cellData.isMoveable = (cellType == 0 || cellType == 4);
                    cellData.canObstacleAttack = (cellTypeData[y, x] == 1 || cellTypeData[y, x] == 2);
                    
                    cells[y, x] = newCell.GetComponent<Cell>();
                    cells[y, x].Init(cellData, battleManager);
                }
            }
            return cells;
        }
        
        private void DestroyGrid()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        private int[,] GenerateCellTypeData()
        {
            int rnd;
            var grid = new int[verticalCount, horizontalCount];
            
            Random.InitState(DateTime.Now.Millisecond);
            
            // 随机障碍物
            for (int i = 0; i < verticalCount; i++)
            {
                for (int j = 0; j < horizontalCount; j++)
                {
                    rnd = UnityEngine.Random.Range(0, 101);
                    if(rnd <= 85)
                        grid[i, j] = 0; // 草地
                    else if(rnd <= 90)
                        grid[i, j] = 1; // 石头
                    else if(rnd <= 100)
                        grid[i, j] = 2; // 树林
                }
            }
            
            // 设置起点和终点
            int col = Random.Range(-3, horizontalCount + 3);
            int brightPosY = Random.Range(0, verticalCount);
            if (col > 2 && col < horizontalCount - 2)
            {
                if (verticalCount >= 5)
                {
                    int startY = Random.Range(-5, 5);
                    int endY = Random.Range(verticalCount - 5, verticalCount);
                    startY = Math.Clamp(startY, 0, verticalCount);
                    endY = Math.Clamp(endY, 0, verticalCount);
                    
                    for (int v = startY; v < endY; v++)
                    {
                        if (v == brightPosY)
                        {
                            grid[v, col] = 4;
                            // 确保桥的两边没有阻挡
                            grid[v, col - 1] = 0;
                            grid[v, col + 1] = 0;
                        }
                        else
                        {
                            grid[v, col] = 3;
                        }
                    }
                }
            }

            return grid;
        }

        public void SaveCellTypeData()
        {
            SaveCellTypeData(_cellTypes);
        }
        
        public void SaveCellTypeData(int[,] cellTypeData)
        {
            // file path
            string path = Application.dataPath + GlobalSetting.CellDataPath;
            // write
            using var sw = new StreamWriter(path);
            sw.WriteLine(verticalCount);
            sw.WriteLine(horizontalCount);
            for (int i = 0; i < verticalCount; i++)
            {
                for (int j = 0; j < horizontalCount; j++)
                {
                    sw.Write(cellTypeData[i, j]);
                    if (j < horizontalCount - 1)
                    {
                        sw.Write(" ");
                    }
                }
                sw.WriteLine();
            }
        }

        public int[,] LoadCellTypeDa()
        {
            // file path
            string path = Application.dataPath + GlobalSetting.CellDataPath;
            // read
            using (var sr = new StreamReader(path))
            {
                verticalCount = int.Parse(sr.ReadLine());
                horizontalCount = int.Parse(sr.ReadLine());
                var grid = new int[verticalCount, horizontalCount];
                for (int i = 0; i < verticalCount; i++)
                {
                    string[] line = sr.ReadLine().Split(' ');
                    for (int j = 0; j < horizontalCount; j++)
                    {
                        grid[i, j] = int.Parse(line[j]);
                    }
                }

                return grid;
            }
        }
    }
}