using System.Text;
using System.Text.RegularExpressions;
using ConsoleSystem;
using Entity;
using GamePlaySystem.DeploySystem;
using GamePlaySystem.FactionSystem;
using UnityEngine;

namespace GamePlaySystem.LevelData
{
    public interface ILevelDataProcessor
    {
        void LoadLevelData(string levelData);
    }
    
    public class LevelDataProcessor : ILevelDataProcessor
    {
        private readonly string labelParser;

        public LevelDataProcessor(string labelParser)
        {
            this.labelParser = labelParser;
        }

        public void LoadLevelData(string levelData)
        {
            // TODO 修改为更优秀的解析方式
            // 用于标签解析的字符串
            MyConsole.Print($"标签解析正则表达式：{labelParser}");

            string[] data = levelData.Split('\n');

            string dataPartType = null;
            int labelStartRow = 0;

            for (int curRow = 0; curRow < data.Length; curRow++)
            {
                if (string.IsNullOrEmpty(data[curRow])) continue;
                if (data[curRow].StartsWith("#")) continue;

                Regex labelRegex = new Regex(labelParser);
                Match match = labelRegex.Match(data[curRow]);
                if (!match.Success) continue;
                
                // 如果匹配到左标签
                if (dataPartType == null)
                {
                    dataPartType = match.Value.Trim();
                    labelStartRow = curRow;
                }
                // 如果匹配到右标签
                else if (dataPartType == match.Value.Trim())
                {
                    // 解析数据
                    if (curRow == labelStartRow + 1)
                    {
                        Debug.LogWarning("标签内数据为空");
                        continue;
                    }

                    StringBuilder packageData = new StringBuilder();
                    for (int j = labelStartRow + 1; j < curRow; j++)
                    {
                        packageData.Append(data[j]);
                        if (j < curRow - 1) packageData.Append("\n"); // 最后一行数据不需要换行
                    }
                    dataPartType = dataPartType.Trim();
                    switch (dataPartType)
                    {
                        case "[Gold]":
                            PlayerData.Gold.Value = int.Parse(packageData.ToString());
                            break;
                        case "[Terrain Data]":
                            ProcessTerrainData(packageData.ToString());
                            break;
                        case "[Unit Data]":
                            ProcessEnemyData(packageData.ToString());
                            break;
                        default:
                            Debug.LogError("未识别的标签类型：" + dataPartType);
                            break;
                    }
                    // 重置数据类型
                    dataPartType = null;
                }
                // 格式错误
                else
                {
                    Debug.LogError("标签未闭合 或 标签嵌套");
                }
            }
        }
        
        private void ProcessTerrainData(string terrainData)
        {
            var entityFactory = ServiceLocator.Get<IEntityFactory>();
            // 解析地形数据
            var rows = terrainData.Split('\n');
            // 需要与读取的数据垂直翻转
            int maxH = rows.Length - 1;
            for (int y = 0; y < rows.Length; y++)
            {
                if (string.IsNullOrEmpty(rows[y])) continue;
                if (rows[y].StartsWith("#")) continue;

                var cols = rows[y].Split(',');
                for (int x = 0; x < cols.Length; x++)
                {
                    // 解析每个格子的数据
                    if (int.TryParse(cols[x], out var cellIndex))
                    {
                        if (cellIndex == 0) continue; // 虚空
                        entityFactory.CreateTile((TileType)cellIndex, new Vector2(x, maxH - y));
                        // EntityFactory.Instance.CreateEntity(cellIndex, new Vector2(x, maxH - y));
                    }
                }
            }
        }

        private void ProcessEnemyData(string unitData)
        {
            var entityFactory = ServiceLocator.Get<IEntityFactory>();
            // 解析单位数据
            string[] data = unitData.Split('\n');
            foreach (var dataRead in data)
            {
                if (string.IsNullOrEmpty(dataRead)) continue;
                if (dataRead.StartsWith("#")) continue;

                var cols = dataRead.Split(',');
                if (cols.Length < 3)
                {
                    Debug.LogError("单位数据格式错误: " + dataRead);
                    return;
                }

                if (!int.TryParse(cols[0], out int entityIndex) || !int.TryParse(cols[1], out int x) || !int.TryParse(cols[2], out int y))
                {
                    Debug.LogError("单位数据解析失败: " + dataRead);
                    return;
                }
                entityFactory.CreateCharacter(entityIndex,new Vector2(x, y), FactionType.Enemy);   
                // EntityFactory.Instance.CreateCharacter(FactionType.Hostile, entityIndex, new Vector2(x, y));
            }
        }
    }
}