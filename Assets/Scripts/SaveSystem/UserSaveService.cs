using System;
using System.Collections.Generic;
using System.IO;
using Data;
using Utility.SerializeTool;

namespace SaveSystem
{
    /// 用户存档
    [Serializable]
    public class UserSave
    {
        public string saveFileName;
        /// <summary>
        /// 存档基本信息
        /// </summary>
        public string saveName;        // 存档名
        public string saveTime;        // 存档时间
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
    
    public interface ISavable
    {
        public void OnSave(UserSave userSave);
        public void OnLoad(UserSave userSave);
    }

    public interface IUserSaveService
    {
        // UserSave GetUserSave();
        public List<UserSave> GetAllUserSaves();
        void Load(UserSave save);
        void Save(UserSave save);
        void QuickSave();
        UserSave CreateNewSave(string saveName);
        void DeleteSave(string saveName);
    }
    
    /// <summary>
    /// 暂时牺牲了一点安全性（反正是单机游戏），直接明文存储
    /// </summary>
    public class UserSaveService : IUserSaveService
    {
        private readonly string _rootPath;
        private readonly ISerializeTool _serializeTool;
        private readonly List<UserSave> _userSaves = new();  // 所有存档
        
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
        
        public void AddSaveSubscribe(ISavable savable)
        {
            _saveSubscribes.Add(savable);
        }
        
        public void RemoveSaveSubscribe(ISavable savable)
        {
            _saveSubscribes.Remove(savable);
        }

        public void Load(UserSave save)
        {
            // 通知订阅者
            foreach (var subscribe in _saveSubscribes) {
                subscribe.OnLoad(save);
            }
        }

        public void Save(UserSave save)
        {
            // 通知订阅者
            foreach (var subscribe in _saveSubscribes) {
                subscribe.OnSave(save);
            }
            // 序列化
            var savePath = Path.Combine(_rootPath, save.saveFileName);
            // 保存存档
            _serializeTool.Serialize(save, savePath);
        }
        
        public void DeleteSave(UserSave save)
        {
            // 删除存档
            var fileName = save.saveFileName;
            var savePath = Path.Combine(_rootPath, fileName);
            File.Delete(savePath);
        }
        
        public void DeleteSave(string saveName)
        {
            // 删除存档
            var files = GetAllUserSaves();
            foreach (var file in files)
            {
                if (file.saveName == saveName)
                {
                    var fileName = file.saveFileName;
                    var savePath = Path.Combine(_rootPath, fileName);
                    File.Delete(savePath);
                    break;
                }
            }
        }
        public List<UserSave> GetAllUserSaves()
        {
            var files = Directory.GetFiles(_rootPath);
            _userSaves.Clear();
            foreach (var file in files)
            {
                var userSave = _serializeTool.Deserialize<UserSave>(file);
                _userSaves.Add(userSave);
            }
            return _userSaves;
        }

        public void QuickSave()
        {
            CreateNewSave("QuickSave");
        }

        public UserSave CreateNewSave(string saveName)
        {
            // 创建新存档
            var userSave = new UserSave
            {
                saveFileName = $"{saveName}_{DateTime.Now:yyyyMMddHHmmss}.json",
                saveName = saveName,
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                levelUnlock = Array.Empty<int>(),
                levelStars = Array.Empty<int>(),
                exp = 0,
                level = 0,
                maxPlayerParty = 0,
                characterProperties = new List<CharacterProperty>()
            };
            // 保存存档
            Save(userSave);
            return userSave;
        }
    }
}