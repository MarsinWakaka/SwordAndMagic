using System;
using System.Collections.Generic;
using Entity;
using GamePlaySystem.TileSystem;
using UnityEngine;

namespace GamePlaySystem.Controller.AI.AIDecisionResource
{
    public class ScoreHeatMapCooker
    {
        private readonly CharacterManager characterManager;
        private readonly TileManager _tileManager;
        public ScoreHeatMapCooker(CharacterManager characterManager, TileManager tileManager)
        {
            this.characterManager = characterManager;
            _tileManager = tileManager;
        }

        // TODO 将其作为角色的参数，从而定义不同角色的行为偏好
        // TODO 例如：坦克更倾向于冲向敌人，法师更倾向于远离敌人，治疗更倾向于靠近友军
        // 尽量靠近敌人
        private const float HostileScoreFactor = 100;
        private const int HostileImpactRange = 5;
        // 尽量远离友军
        private const float FriendlyScoreFactor = -50;
        private const int FriendlyImpactRange = 3;
        
        private int maxX;
        private int maxY;
        private Tile[,] tiles;
        private Dictionary<TileType, TileData> tileDict;
        private float[,] scoreHeatMap;
        
        public float[,] Cook(Character decider)
        {
            // 初始化
            tiles = _tileManager.GetTiles();
            tileDict = _tileManager.GetTileDict();
            maxX = tiles.GetLength(0);
            maxY = tiles.GetLength(1);
            scoreHeatMap = new float[maxX, maxY];
            // 计算危险度
            foreach (var tile in tiles)
            {
                if (tile == null) continue;
                var pos = tile.transform.position;
                scoreHeatMap[(int)pos.x, (int)pos.y] = tileDict[tile.TileType].score;
            }
            CookRisk(decider);
            return scoreHeatMap;
        }
        
        private void CookRisk(Character decider)
        {
            var factionType = decider.Faction.Value;
            var hostileFaction = 1 - factionType;
            var hostileUnits = characterManager.GetUnitsByFaction(hostileFaction);
            // 计算敌人危险度
            foreach (var unit in hostileUnits)
            {
                var unitPos = unit.transform.position;
                var unitX = (int) unitPos.x;
                var unitY = (int) unitPos.y;
                // TODO 将HostileImpactRange，HostileScoreFactor作为角色的参数，从而定义不同角色的行为偏好(天才！)
                ApplyRiskByCircleLinearDecay(unitX, unitY, HostileImpactRange, HostileScoreFactor); //后续考虑根据不同职业制作不同危险度衰减函数
                // 角色位置不可到达，所以需要将其分数置为最小值
                scoreHeatMap[unitX, unitY] = float.MinValue / 2;    // 防止溢出
            }
            var friendlyUnits = characterManager.GetUnitsByFaction(factionType);
            foreach (var unit in friendlyUnits)
            {
                if (unit == decider) continue;
                var unitPos = unit.transform.position;
                var unitX = (int) unitPos.x;
                var unitY = (int) unitPos.y;
                ApplyRiskByCircleLinearDecay(unitX, unitY, FriendlyImpactRange, FriendlyScoreFactor); //后续考虑根据不同职业制作不同危险度衰减函数
                scoreHeatMap[unitX, unitY] = float.MinValue / 2;    // 角色位置不可达
            }
        }
        
        // 施加线性衰减的危险度
        private void ApplyRiskByCircleLinearDecay(int x, int y, int range, in float riskFactor)
        {
            if (range == 0) return;
            var left = Math.Max(0, x - range);
            var right = Math.Min(maxX - 1, x + range);
            var bottom = Math.Max(0, y - range);
            var top = Math.Min(maxY - 1, y + range);
            for (var xo = left; xo <= right; xo++)
            {
                for (var yo = bottom; yo <= top; yo++)
                {
                    var dist = Mathf.Abs(xo - x) + Mathf.Abs(yo - y);
                    if (HasTile(xo, yo) && dist <= range)
                    {
                        scoreHeatMap[xo, yo] += riskFactor * (range - dist) / range;
                    }
                }
            }
        }
        
        private bool CheckBorder(int x, int y) => x >= 0 && x < maxX && y >= 0 && y < maxY;
        
        private bool HasTile(int x, int y) => tiles[x, y] != null;
    }
}