using System;
using System.Collections.Generic;
using Utility.SerializeTool;

namespace SaveSystem
{
    /// 用户存档
    [Serializable]
    public class UserSave
    {
        public string saveName;        // 存档名
        public int coin;                // 用于角色解锁与升级
        /// <summary>
        /// 该存档当前的关卡数据
        /// </summary>
        public int[] levelUnlock;        // 已解锁关卡ID
        public int[] levelStars;         // 获得的该关卡的最高星级
        // public List<LevelSave> LevelSaves;
        /// <summary>
        /// 以下属性用于成长系统
        /// </summary>
        public int[] characterLevel;    // 索引为角色ID,数值为对应的角色等级
        public int maxPlayerParty;      // 玩家最大队伍数量
    }
    
    public interface IUserSaveService
    {
        UserSave GetUserSave();
        void LoadUserSave(string saveName);
        void SaveCurSave(UserSave userSave);
        void CreateNewSave(string saveName);
        void DeleteSave(string saveName);
    }
    
    /// <summary>
    /// 暂时牺牲了一点安全性（反正是单机游戏），直接明文存储
    /// </summary>
    public class UserSaveService : IUserSaveService
    {
        private string _rootPath;
        private UserSave _curSave;
        private List<UserSave> _userSaves;  // 所有存档
        private ISerializeTool _serializeTool;

        public UserSaveService(string rootPath, ISerializeTool serializeTool)
        {
            _rootPath = rootPath;
            _serializeTool = serializeTool;
        }
        
        public UserSave GetUserSave()
        {
            return _curSave;
        }
        
        public void LoadUserSave(string saveName)
        {
            // 反序列化
        }
        
        public void SaveCurSave(UserSave userSave)
        {
            // 序列化
            // 保存存档
        }
        
        public void CreateNewSave(string saveName)
        {
            // 创建新存档
            // 保存存档
        }
        
        public void DeleteSave(string saveName)
        {
            // 删除存档
        }
    }
}