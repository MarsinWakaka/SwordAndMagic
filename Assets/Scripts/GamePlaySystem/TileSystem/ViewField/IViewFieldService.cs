using System.Collections.Generic;
using UnityEngine;

namespace GamePlaySystem.TileSystem.ViewField
{
    public interface IViewFieldService
    {
        public List<Vector2Int> GetViewField(int startX, int startY, int viewRange);
        
        public HashSet<int> GetViewFieldSets(int startX, int startY, int viewRange);
    }
}