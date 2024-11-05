using System;
using System.Collections.Generic;
using Data;
using Entity;

namespace SaveSystem
{
    /// 用户存档
    [Serializable]
    public class UserSave
    {
        public string saveName;        // 存档名
        /// <summary>
        /// 该存档当前的关卡数据
        /// </summary>
        public int[] levelUnlock;        // 已解锁关卡ID
        public int[] levelStars;         // 获得的该关卡的最高星级
        // public List<LevelSave> LevelSaves;
        /// <summary>
        /// 角色数据
        /// </summary>
        public int exp;                     // 用于角色解锁与升级
        public int level;                   // 角色等级(这是队伍角色共享的)
        public int partySize = 0;
        public int maxPlayerParty;      // 玩家最大队伍数量
        public List<CharacterProperty> characterProperties;    // 已有角色的数据
    }
}