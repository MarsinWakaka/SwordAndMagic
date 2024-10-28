// using System.Linq;
// using GamePlaySystem.ScopeSystem;
// using UnityEngine;
//
// namespace GamePlaySystem.Scope
// {
//     public interface IScope
//     {
//         bool IsInScope(ScopeParam param);
//     }
//     
//     public class CircleScope : IScope
//     {
//         public bool IsInScope(ScopeParam param)
//         {
//             return ScopeUtilities.InManhattanScope(param);
//         }
//     }
//     
//     public class CustomScope : IScope
//     {
//         // 自定义施法范围,偏移值以目标点为基准
//         private readonly Vector2[] _offsets;
//         
//         public CustomScope(Vector2[] offsets)
//         {
//             _offsets = offsets;
//         }
//         
//         public bool IsInScope(ScopeParam param)
//         {
//             return _offsets.Any(offset => param.pointA + offset == param.pointB);
//         }
//     }
// }