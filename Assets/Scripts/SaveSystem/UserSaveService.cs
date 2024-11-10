using System;
using System.Collections.Generic;
using System.IO;
using Data;
using GamePlaySystem;
using Utility.SerializeTool;

namespace SaveSystem
{
    /// 用户存档
    [Serializable]
    public class UserData
    {
        public string saveFileName;    // 存档文件名
        public string saveName;        // 存档名
        public string saveTime;        // 存档时间
        
        public PlayerData playerData;
    }
    
    public interface ISavable
    {
        public void OnSave(ref UserData userData);
        public void OnLoad(in UserData userData);
    }

    public interface IUserSaveService
    {
        // UserData GetUserSave();
        public void AddSaveSubscribe(ISavable savable);
        public void RemoveSaveSubscribe(ISavable savable);
        public List<UserData> GetAllUserSaves();
        void Load(UserData data);
        void Save(UserData data);
        void QuickSave();
        UserData CreateNewSave(string saveName);
        void DeleteSave(UserData data);
    }
    
    /// <summary>
    /// 暂时牺牲了一点安全性（反正是单机游戏），直接明文存储
    /// </summary>
    public class UserSaveService : IUserSaveService
    {
        private readonly string _rootPath;
        private readonly ISerializeTool _serializeTool;
        private readonly List<UserData> _userSaves = new();  // 所有存档
        
        private readonly List<ISavable> _saveSubscribes = new();
        public UserSaveService(string rootPath, ISerializeTool serializeTool)
        {
            _rootPath = rootPath;
            _serializeTool = serializeTool;
            // 检查存档目录是否存在
            if (!Directory.Exists(_rootPath)) {
                Directory.CreateDirectory(_rootPath);
            }
        }
        
        public void AddSaveSubscribe(ISavable savable) {
            _saveSubscribes.Add(savable);
        }
        
        public void RemoveSaveSubscribe(ISavable savable) {
            _saveSubscribes.Remove(savable);
        }

        public void Load(UserData data) {
            // 通知订阅者
            foreach (var subscribe in _saveSubscribes) {
                subscribe.OnLoad(in data);
            }
        }

        public void Save(UserData data) {
            // 通知订阅者
            foreach (var subscribe in _saveSubscribes) {
                subscribe.OnSave(ref data);
            }
            // 序列化
            var savePath = Path.Combine(_rootPath, data.saveFileName);
            data.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // 保存存档
            _serializeTool.Serialize(data, savePath);
        }
        
        public void DeleteSave(UserData data) {
            // 删除存档
            var fileName = data.saveFileName;
            var savePath = Path.Combine(_rootPath, fileName);
            File.Delete(savePath);
        }
        
        public List<UserData> GetAllUserSaves() {
            var files = Directory.GetFiles(_rootPath);
            _userSaves.Clear();
            foreach (var file in files)
            {
                var userSave = _serializeTool.Deserialize<UserData>(file);
                _userSaves.Add(userSave);
            }
            return _userSaves;
        }

        public void QuickSave()
        {
            CreateNewSave("QuickSave");
        }

        public UserData CreateNewSave(string saveName) {
            // 创建新存档
            var userSave = new UserData
            {
                saveFileName = $"{saveName}_{DateTime.Now:yyyyMMddHHmmss}.json",
                saveName = saveName,
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                playerData = PlayerManager.Instance.playerData
            };
            // 保存存档
            Save(userSave);
            return userSave;
        }
    }
}