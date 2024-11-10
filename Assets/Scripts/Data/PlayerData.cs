using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public string teamName;        // 队伍名
        public int teamLevel = 1;          // 队伍等级
        public int teamExp = 0;            // 队伍经验
        public int teamMaxExp = 1000;         // 队伍升级所需经验
        public int teamGold = 100;           // 队伍金币
        
        public int curPartySize;
        public int maxPlayerParty = 3;      // 玩家最大队伍数量
        public List<CharacterProperty> characterProperties;    // 已有角色的数据
        public int[] levelUnlock;        // 已解锁关卡ID
        public int[] levelStars;         // 获得的该关卡的最高星级
        
        // 大地图信息
        // public MapData mapData;
        
        public void AddExp(int expGet)
        {
            teamExp += expGet;
            if (teamExp >= teamMaxExp) {
                teamExp -= teamMaxExp;
                teamLevel++;
                // 经验公式
                teamMaxExp = 1000 * (teamLevel / 3 + 1);
            }
        }
    }
}