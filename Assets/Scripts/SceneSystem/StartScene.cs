﻿using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSystem
{
    public class StartScene : IScene
    {
        private const string SceneName = "StartScene";

        public void Init()
        {
            // 初始化开始场景
            if (SceneManager.GetActiveScene().name == SceneName)
            {
                UIManager.Instance.PushPanel(PanelType.StartPanel, null);
            }
            else
            {
                var loadHandle = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
                if (loadHandle != null)
                    loadHandle.completed += operation =>
                    {
                        UIManager.Instance.PushPanel(PanelType.StartPanel, null);
                    };
                else
                {
                    Debug.LogError($"LoadSaveByName Scene Failed，请检查场景名字是否正确 {SceneName}");
                }
            }
        }
        
        public void Release()
        {
            // 释放场景
            UIManager.Instance.ClearPanel();    // 清空UI（主菜单面板）
        }
    }
}