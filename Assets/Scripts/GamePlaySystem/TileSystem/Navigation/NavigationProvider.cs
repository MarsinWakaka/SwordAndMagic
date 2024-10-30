using System.Collections.Generic;
using UnityEngine;

namespace GamePlaySystem.TileSystem.Navigation
{
    public class PathNode
    {
        public int PosX;
        public int PosY;
        public PathNode FromNode;
        public int Cost;

        public PathNode(int posX, int posY, PathNode fromNode, int cost)
        {
            PosX = posX;
            PosY = posY;
            FromNode = fromNode;
            Cost = cost;
        }
    }
    
    public class NavigationProvider
    {
        private readonly ITileManager tileManager;
        
        public NavigationProvider(ITileManager tileManager)
        {
            this.tileManager = tileManager;
        }

        private readonly int[,] dir = {{0, 1}, {0, -1}, {-1, 0}, {1, 0}}; // 四联通
        
        /// <summary>
        /// 获取指定位置的所有可到达位置（目前只考虑四联通,考虑了不同地形消耗的影响）
        /// </summary>
        public List<PathNode> GetReachablePositions(int startX, int startY, int range)
        {
            var tiles = tileManager.GetTiles();
            if (tiles[startX, startY] == null) return new List<PathNode>();
            var tileDict = tileManager.GetTileDict();
            
            var path = new List<PathNode>();
            var startNode = new PathNode(startX, startY, null, 0);
            var fromNodes = new Queue<PathNode>();
            var closeDict = new Dictionary<int, PathNode>(); // 错位存储XY坐标,每个占10bit
            fromNodes.Enqueue(startNode);
            closeDict.Add((startX << 10) + startY, startNode); // 位运算优先级低于加减法
            path.Add(startNode);
            while (fromNodes.Count > 0)
            {
                var fromNode = fromNodes.Dequeue();
                for (var i = 0; i < 4; i++)
                {
                    var curPosX = fromNode.PosX + dir[i, 0];
                    var curPosY = fromNode.PosY + dir[i, 1];
                    // 此格子能否通过上一节点到达
                    if (!tileManager.InBorder(curPosX, curPosY)) continue; // 1、在边界内
                    var curTile = tiles[curPosX, curPosY];
                    if (curTile == null) continue;                      // 2、不是空格子 
                    if (tileDict[curTile.TileType].canBlockMove) continue; // 3、不是阻挡
                    var fromTile = tiles[fromNode.PosX, fromNode.PosY];
                    var curCost = fromNode.Cost + tileDict[fromTile.TileType].leaveCost;
                    if (curCost > range) continue;                          // 4、有行动力离开此格子
                    // 此格子是否已经被访问过
                    // 如果后来的节点到达此格子的代价更小，则更新此格子的代价(需要将此格子重新加入队列)
                    if (closeDict.TryGetValue((curPosX << 10) + curPosY, out var oldCurNode)) { 
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
                        closeDict.Add((curNode.PosX << 10) + curNode.PosY, curNode);
                        path.Add(curNode);
                    }
                }
            }
            Debug.Log($"GetReachablePositions: {path.Count}");
            return path;
        }
    }
}