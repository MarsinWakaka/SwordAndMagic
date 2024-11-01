using System;
using System.Collections.Generic;
using GamePlaySystem.TileSystem.Navigation;
using UnityEngine;

namespace GamePlaySystem.TileSystem.ViewField
{
    public class SimpleViewField : IViewFieldService
    {
        private readonly TileManager _tileManager;
        public SimpleViewField(TileManager tileManager)
        {
            _tileManager = tileManager;
        }
        
        public List<Vector2Int> GetViewField(int startX, int startY, int viewRange)
        {
            var tiles = _tileManager.GetTiles();
            var width = tiles.GetLength(0);
            var height = tiles.GetLength(1);
            var left = Math.Max(0, startX - viewRange);
            var bottom = Math.Max(0, startY - viewRange);
            var right = Math.Min(width - 1, startX + viewRange);
            var top = Math.Min(height - 1, startY + viewRange);
            var viewField = new List<Vector2Int>();
            for (var x = left; x <= right; x++){
                for (var y = bottom; y <= top; y++){
                    if (Math.Abs(startX - x) + Math.Abs(startY - y) > viewRange) continue; // 曼哈顿视野。
                    if (_tileManager.HasTile(x, y))
                        viewField.Add(new Vector2Int(x, y));
                }
            }
            return viewField;
        }
        
        public HashSet<int> GetViewFieldSets(int startX, int startY, int viewRange)
        {
            var tiles = _tileManager.GetTiles();
            var width = tiles.GetLength(0);
            var height = tiles.GetLength(1);
            var left = Math.Max(0, startX - viewRange);
            var bottom = Math.Max(0, startY - viewRange);
            var right = Math.Min(width - 1, startX + viewRange);
            var top = Math.Min(height - 1, startY + viewRange);
            var viewField = new HashSet<int>();
            for (var x = left; x <= right; x++){
                for (var y = bottom; y <= top; y++){
                    if (Math.Abs(startX - x) + Math.Abs(startY - y) > viewRange) continue; // 曼哈顿视野。
                    if (_tileManager.HasTile(x, y))
                        viewField.Add(NavigationService.GetIndexKey(x, y));  // 二维坐标转一维坐标
                        // viewField.Add(x * height + y);  // 二维坐标转一维坐标
                }
            }
            return viewField;
        } 
    }
}