using System.IO;
using Configuration;
using GamePlaySystem;
using GamePlaySystem.CharacterClassSystem;
using ResourcesSystem;
using SaveSystem;
using SceneSystem;
using UnityEngine;
using Utility.SerializeTool;
using Utility.SerializeTool.Concrete;
using Utility.Singleton;
using AddressableManager = ResourcesSystem.AddressableManager;

/// <summary>
/// 顶级应用程序类，用于管理游戏的各个模块
/// </summary>
public sealed class ApplicationRoot : SingletonMono<ApplicationRoot>
{
    protected override void OnAwake()
    {
        // 初始化全局服务【】
        var configPath = "Assets/ConfigData.json";//Path.Combine(Application.streamingAssetsPath, "ConfigData.json");
        var serializeTool = new JsonSerializeTool();
        var configService = new ConfigService(configPath, serializeTool);
        var resourceManager = new AddressableManager();
        var rootPath = Path.Combine(Application.persistentDataPath, configService.ConfigData.saveDataPath);
        var userSaveService = new UserSaveService(rootPath, serializeTool);
        var characterClassManager = new CharacterClassManager(configService.ConfigData.characterClassDataTag, resourceManager);
        ServiceLocator.Register<ISerializeTool>(serializeTool);
        ServiceLocator.Register<IConfigService>(configService);
        ServiceLocator.Register<IResourceManager>(resourceManager);
        ServiceLocator.Register<IUserSaveService>(userSaveService);
        ServiceLocator.Register<CharacterClassManager>(characterClassManager);
    }

    private void Start()
    {
        PlayerManager.Instance.Initialize();
        GameSceneManager.LoadScene(new StartScene());
    }

    // private void OnApplicationQuit()
    // {
    //     // TODO 保存用户存档
    //     var userSaveService = ServiceLocator.Get<IUserSaveService>();
    //     userSaveService.QuickSave();
    // }
}
