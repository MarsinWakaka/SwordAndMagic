using System;
using System.Collections.Generic;
using GamePlaySystem.LevelDataSystem;
using UnityEngine;

namespace GamePlaySystem._EXP.MainMap
{
    [Serializable]
    public class MapData
    {
        public string mapName;
        public string mapDescription;
        public Vector2Int mapSize;
        public List<LevelData> mapData;   // 老规矩，压缩成一维数组
        
        // 多地图传送点（暂时不添加）
        // public int mapID;
        // public Vector2Int mapStartPoint;
        // public int endPointMapID;
        // public Vector2Int mapEndPoint;
    }
}