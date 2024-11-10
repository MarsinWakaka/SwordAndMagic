using System;
using System.Collections.Generic;
using Utility;

namespace GamePlaySystem.LevelDataSystem
{
    [Serializable]
    public class LevelData
    {
        public int levelID;
        public string levelName;
        public string levelDescription;
        public int levelDifficulty;
        public int levelGold;
        public static readonly BindableProperty<int> Gold = new();
        // 32位bit，从后往前
        // 01-10bit：X坐标
        // 11-20bit：Y坐标
        // 21-30bit：瓦片类型
        
        // 11-20bit：瓦片上的物品
        // 21-30bit：瓦片上的单位ID
        // 31bit: 单位阵营
        // 32bit：场景效果（例如可部署、不可部署）
        // 每10位分别代表(X,Y,TileType)
        public List<int> gridData;
        public List<int> gridAppendData;
        public int expWhileWin;
    }
}