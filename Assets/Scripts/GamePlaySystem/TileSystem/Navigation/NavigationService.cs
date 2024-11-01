using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamePlaySystem.TileSystem.Navigation
{
    public class PathNode
    {
        public int PosX;
        public int PosY;
        public PathNode FromNode;
        public int Cost;    // 从起点到此节点的代价

        public PathNode(int posX, int posY, PathNode fromNode, int cost)
        {
            PosX = posX;
            PosY = posY;
            FromNode = fromNode;
            Cost = cost;
        }
        
        public override string ToString() => $"({PosX},{PosY})";
    }
    
    public interface INavigationService
    {
        List<PathNode> GetReachablePositions(int startX, int startY, int range);
        
        Dictionary<int, PathNode> GetReachablePositionDict(int startX, int startY, int range);
    }
    
    public class NavigationService : INavigationService
    {
        private readonly TileManager tileManager;
        
        public NavigationService(TileManager tileManager)
        {
            this.tileManager = tileManager;
        }

        private readonly int[] dir = {0, 1, 0, -1, -1, 0, 1, 0}; // 四联通
        
        /// <summary>
        /// 获取指定位置的所有可到达位置（目前只考虑四联通,考虑了不同地形消耗的影响）
        /// 注意：路径的起点的FromNode为null
        /// </summary>
        public List<PathNode> GetReachablePositions(int startX, int startY, int range)
        {
            return GetReachablePositionDict(startX, startY, range).Values.ToList();
        }
        
        /// <summary>
        /// 获取指定位置的所有可到达位置（目前只考虑四联通,考虑了不同地形消耗的影响）
        /// 注意：路径的起点的FromNode为null
        /// </summary>
        public Dictionary<int ,PathNode> GetReachablePositionDict(int startX, int startY, int range)
        {
            var tiles = tileManager.GetTiles();
            if (tiles[startX, startY] == null) return new Dictionary<int, PathNode>();
            var tileDict = tileManager.GetTileDict();
            
            var startNode = new PathNode(startX, startY, null, 0);
            var fromNodes = new Queue<PathNode>();
            var path = new Dictionary<int, PathNode>(); // 错位存储XY坐标,每个占10bit
            fromNodes.Enqueue(startNode);
            path.Add(GetIndexKey(startX, startY), startNode); // 位运算优先级低于加减法
            while (fromNodes.Count > 0)
            {
                var fromNode = fromNodes.Dequeue();
                for (var i = 0; i < 4; i++)
                {
                    var curPosX = fromNode.PosX + dir[i * 2];
                    var curPosY = fromNode.PosY + dir[i * 2 + 1];
                    // 此格子能否通过上一节点到达
                    if (!tileManager.InBorder(curPosX, curPosY)) continue; // 1、在边界外
                    var curTile = tiles[curPosX, curPosY];
                    if (curTile == null) continue;                       // 2、是空格子 
                    if (tileDict[curTile.TileType].canBlockMove) continue;  // 3、阻碍移动
                    if (curTile.IsBlocked) continue;                        // 4、没有被物体阻挡
                    var fromTile = tiles[fromNode.PosX, fromNode.PosY];
                    var curCost = fromNode.Cost + tileDict[fromTile.TileType].leaveCost;
                    if (curCost > range) continue;                          // 5、没有有行动力离开此格子
                    // 此格子是否已经被访问过
                    // 如果后来的节点到达此格子的代价更小，则更新此格子的代价(需要将此格子重新加入队列)
                    if (path.TryGetValue(GetIndexKey(curPosX, curPosY), out var oldCurNode)) { 
                        if (oldCurNode.Cost > curCost) {
                            oldCurNode.Cost = curCost;
                            oldCurNode.FromNode = fromNode;
                            fromNodes.Enqueue(oldCurNode);
                        }
                    }
                    // 否则，将此格子加入队列
                    else
                    {
                        var curNode = new PathNode(curPosX, curPosY, fromNode, curCost);
                        fromNodes.Enqueue(curNode);
                        path.Add(GetIndexKey(curPosX, curPosY), curNode);
                    }
                }
            }
            Debug.Log($"GetReachablePositions: {path.Count}");
            return path;
        }

        public static int GetIndexKey(int x, int y) => (x << 10) + y;
        
        public static void GetXYFromIndexKey(int key, out int x, out int y)
        {
            x = key >> 10;
            y = key & 0x3FF;
        }
    }
}