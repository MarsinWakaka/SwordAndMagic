using System;
using UnityEngine;

namespace GamePlaySystem._EXP.MainMap
{
    [Serializable]
    public class MapGridData
    {
        // 利用Vector2Int来表示地图坐标(拼凑为字符串 grid_x_y 的格式，然后去序列化对应的地图数据加载进入)
        public Vector2Int gridPosition;
        public bool hasVisited; // 是否已经访问过
        public string mapGridName;
        public string mapGridDescription;
    }
}