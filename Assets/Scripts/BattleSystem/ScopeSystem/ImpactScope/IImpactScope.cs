using System;
// using BattleSystem.Scope;
using UnityEngine;

namespace BattleSystem.ScopeSystem.ImpactScope
{
    
    public interface IImpactScope
    {
        Vector2Int[] GetImpactScope(ScopeParam param);
    }
    
    // public class ManhattanImpactScope : IImpactScope
    // {
    //     public Vector2Int[] GetImpactScope(ScopeParam param)
    //     {
    //         if (param.radius < 0)
    //             throw new Exception("Radius must be greater than 0");
    //         if (param.radius == 0)
    //             return new[]{ param.pointB };
    //         
    //         var targetPos = param.pointB;
    //         var radius = param.radius;
    //         
    //         var upper = targetPos.y + radius;
    //         var lower = targetPos.y - radius;
    //         var left = targetPos.x - radius;
    //         var right = targetPos.x + radius;
    //
    //         // 1 5 13 25 41
    //         var count = 2 * radius * (radius + 1) + 1;
    //         var impactScope = new Vector2Int[count];
    //         
    //         var tempVector = new Vector2Int();
    //         
    //         // TODO 优化，执行效率能快一倍
    //         for (var y = upper; y >= lower; y--)
    //         {
    //             for (var x = left; x <= right; x++)
    //             {
    //                 tempVector.x = x;
    //                 tempVector.y = y;
    //                 if (ScopeUtilities.InManhattanScope(targetPos, tempVector, radius))
    //                     impactScope[--count] = new Vector2Int(x, y);
    //             }
    //         }
    //
    //         return impactScope;
    //     }
    // }
    //
    // public class CustomImpactScope : IImpactScope
    // {
    //     private Vector2Int[] _offsets;
    //     
    //     public CustomImpactScope(Vector2Int[] offsets)
    //     {
    //         _offsets = offsets;
    //     }
    //     
    //     public Vector2Int[] GetImpactScope(ScopeParam param)
    //     {
    //         var targetPos = param.pointB;
    //         var impactScope = new Vector2Int[_offsets.Length];
    //         for (var i = 0; i < _offsets.Length; i++)
    //         {
    //             impactScope[i] = targetPos + _offsets[i];
    //         }
    //
    //         return impactScope;
    //     }
    // }
}