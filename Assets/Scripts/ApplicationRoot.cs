using System.IO;
using Configuration;
using Entity;
using GamePlaySystem.LevelData;
using ResourcesSystem;
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
    public ConfigData Config { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Register<ISerializeTool>(new JsonSerializeTool());
        LoadConfig(Path.Combine(Application.streamingAssetsPath, "config.json"));
        ServiceLocator.Register<IResourceManager>(new AddressableManager());
        ServiceLocator.Register<IEntityFactory>(new EntityFactoryImpl(Config.characterPrefabPath, Config.tilePrefabPath));
        ServiceLocator.Register<ILevelDataProcessor>(new LevelDataProcessor(Config.levelDataParser));
    }

    private void Start()
    {
        GameSceneManager.LoadScene(new MainScene());
    }

    private void OnApplicationQuit()
    {
        
    }
    
    private void LoadConfig(string path)
    {
        if (File.Exists(path)) {
            Config = ServiceLocator.Get<ISerializeTool>().Deserialize<ConfigData>(path);
        } else {
            Debug.LogError("配置文件不存在: " + path);
        }
    }
}
