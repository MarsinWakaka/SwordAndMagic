// using System;
// using GamePlaySystem.ScopeSystem;
// using UnityEngine;
//
// namespace GamePlaySystem.Scope
// {
//     public static class ScopeUtilities
//     {
//         public static bool InManhattanScope(Vector2 pointA, Vector2 pointB, int radius)
//         {
//             if (radius < 0)
//                 throw new Exception("Radius must be greater than 0");
//             if (radius == 0)
//                 return Mathf.Approximately(pointA.x, pointB.x) && Mathf.Approximately(pointA.y, pointB.y);
//             
//             var deltaPos = pointA - pointB;
//             return (int)Mathf.Abs(deltaPos.x) + Mathf.Abs(deltaPos.y) <= radius;
//         }
//         
//         public static bool InManhattanScope(ScopeParam param)
//         {
//             return InManhattanScope(param.pointA, param.pointB, param.radius);
//         }
//     }
// }