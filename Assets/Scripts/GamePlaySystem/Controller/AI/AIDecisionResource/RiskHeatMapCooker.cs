using System;
using System.Collections.Generic;
using BattleSystem.FactionSystem;
using Entity;
using GamePlaySystem.TileSystem;
using UnityEngine;

namespace GamePlaySystem.Controller.AI.AIDecisionResource
{
    public class RiskHeatMapCooker
    {
        private readonly CharacterManager _characterManager;
        private readonly TileManager _tileManager;
        public RiskHeatMapCooker(CharacterManager characterManager, TileManager tileManager)
        {
            _characterManager = characterManager;
            _tileManager = tileManager;
        }

        private const float HostileRiskFactor = 100;
        private const float FriendlyRiskFactor = -50;
        private const int HostileRiskImpactRange = 5;
        private const int FriendlyRiskImpactRange = 4;
        
        private int maxX;
        private int maxY;
        private Tile[,] tiles;
        private Dictionary<TileType, TileData> tileDict;
        private float[,] riskHeatMap;
        
        public float[,] Cook(FactionType factionType)
        {
            // 初始化
            tiles = _tileManager.GetTiles();
            tileDict = _tileManager.GetTileDict();
            maxX = tiles.GetLength(0);
            maxY = tiles.GetLength(1);
            riskHeatMap = new float[maxX, maxY];
            // 计算危险度
            
            // 危险度初始化
            for (var x = 0; x < maxX; x++)
            {
                for (var y = 0; y < maxY; y++)
                {
                    // 计算危险度
                    riskHeatMap[x, y] = tileDict[tiles[x, y].TileType].risk;
                }
            }
            CookRisk(factionType);
            return riskHeatMap;
        }
        
        private void CookRisk(FactionType factionType)
        {
            var hostileFaction = factionType == FactionType.Player ? FactionType.Enemy : FactionType.Player;
            var hostileUnits = _characterManager.GetUnitsByFaction(hostileFaction);
            // 计算敌人危险度
            foreach (var unit in hostileUnits)
            {
                var unitPos = unit.transform.position;
                var unitX = (int) unitPos.x;
                var unitY = (int) unitPos.y;
                ApplyRiskByCircleLinearDecay(unitX, unitY, HostileRiskImpactRange, HostileRiskFactor); //后续考虑根据不同职业制作不同危险度衰减函数
            }
            var friendlyUnits = _characterManager.GetUnitsByFaction(factionType);
            foreach (var unit in friendlyUnits)
            {
                var unitPos = unit.transform.position;
                var unitX = (int) unitPos.x;
                var unitY = (int) unitPos.y;
                ApplyRiskByCircleLinearDecay(unitX, unitY, FriendlyRiskImpactRange, FriendlyRiskFactor); //后续考虑根据不同职业制作不同危险度衰减函数
            }
        }
        
        // 施加线性衰减的危险度
        private void ApplyRiskByCircleLinearDecay(int x, int y, int range, in float riskFactor)
        {
            if (range == 0) return;
            var left = Math.Max(0, x - range);
            var bottom = Math.Max(0, y - range);
            var right = Math.Min(maxX - 1, x + range);
            var top = Math.Min(maxY - 1, y + range);
            for (var xo = left; xo <= right; xo++)
            {
                for (var yo = top; yo <= bottom; yo++)
                {
                    var dist = Mathf.Abs(xo - x) + Mathf.Abs(yo - y);
                    if (HasTile(xo, yo) && dist <= range)
                    {
                        riskHeatMap[xo, yo] += riskFactor * (range - dist) / range;
                    }
                }
            }
        }
        
        private bool CheckBorder(int x, int y)
        {
            return x >= 0 && x < maxX && y >= 0 && y < maxY;
        }
        
        private bool HasTile(int x, int y) => tiles[x, y] != null;
    }
}