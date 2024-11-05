using System;
using System.Collections.Generic;
using Entity;
using Utility;
using Utility.SerializeTool;

namespace SaveSystem
{
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