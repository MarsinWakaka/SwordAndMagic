using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Singleton;
using GlobalSetting = Configuration.GlobalSetting;

namespace UISystem
{
    /// <summary>
    /// Panel声明的名称 与 枚举类型的名称 保持一致
    /// </summary>
    public enum PanelType
    {
        MainMenusPanel,
        CharacterEmporiumPanel,
        BattlePanel,
        BattleEndPanel,
    }
    
    // public enum PanelOperation
    // {
    //     Push,
    //     Pop,
    //     Clear,
    // }
    
    public class UIManager : SingletonMono<UIManager>
    {
        [SerializeField] private Transform panelRoot;
        
        private readonly Dictionary<PanelType, BasePanel> _panelDict = new ();
        private readonly Stack<BasePanel> _panelStack = new ();
        
        protected override void Awake()
        {
            base.Awake();
            // EventCenter<string>.Instance.AddListener<PanelType, PanelOperation>("UIEvent", HandlePanelEvent);
            PushPanel(PanelType.MainMenusPanel);
        }
        
        // private void HandlePanelEvent(PanelType panelType, PanelOperation panelOperation)
        // {
        //     switch (panelOperation)
        //     {
        //         case PanelOperation.Push:
        //             PushPanel(panelType);
        //             break;
        //         case PanelOperation.Pop:
        //             PopPanel();
        //             break;
        //         case PanelOperation.Clear:
        //             ClearPanel();
        //             break;
        //     }
        // }
        
        private void LoadAllPanel()
        {
            var path = $"{GlobalSetting.UIPanelPath}";
            BasePanel[] panels = Resources.LoadAll<BasePanel>(path);
            foreach (var panel in panels)
            {
                _panelDict.Add(panel.panelType, panel);
                print("加载面板: " + panel.panelType);
            }
            print($"加载完成, 共计：{panels.Length}个");
        }

        private BasePanel LoadPanel(PanelType type)
        {
            var path = $"{GlobalSetting.UIPanelPath}{type}";
            var panelPrefab = Resources.Load<BasePanel>(path);
            if (panelPrefab)
            {
                var panel = Instantiate(panelPrefab, panelRoot);
                _panelDict.Add(panelPrefab.panelType, panelPrefab);
            
                print("加载面板: " + panelPrefab.panelType);
                return panel;
            }
            print("加载失败: " + path);
            return null;
        }

        // /// <summary>
        // /// 异步方法加载Panel
        // /// </summary>
        // /// <param name="panelType"></param>
        // private void LoadPanelAsync(PanelType panelType)
        // {
        //     Addressables.LoadAssetAsync<GameObject>($"{GlobalSetting.UIPanelPath}/{panelType}.prefab").Completed += operation =>
        //     {
        //         if (operation.Status == AsyncOperationStatus.Succeeded)
        //         {
        //             var panelComponent = operation.Result.GetComponent<BasePanel>();
        //             _panelDict.Add(panelType, panelComponent);
        //         }
        //     };
        // }
        
        private BasePanel GetPanel(PanelType panelType)
        {
            if (_panelDict.TryGetValue(panelType, out var panel)) return panel;
            
            return LoadPanel(panelType);
        }
        
        
        #region 功能区
        
        public BasePanel GetCurrentPanel() => _panelStack.Peek();
        
        public void PushPanel(PanelType panelType)
        {
            if (_panelStack.Count > 0) 
                _panelStack.Peek().OnPause();
            var panel = GetPanel(panelType);
            panel.OnEnter();
            _panelStack.Push(panel);
        }
        
        public void PushPanel(PanelType panelType, Action onComplete)
        {
            PushPanel(panelType);
            onComplete?.Invoke();
        }
        
        public void PushPanel(PanelType panelType, Action<BasePanel> onComplete)
        {
            if (_panelStack.Count > 0) 
                _panelStack.Peek().OnPause();
            var panel = GetPanel(panelType);
            panel.OnEnter();
            _panelStack.Push(panel);
            onComplete?.Invoke(panel);
        }
        
        public void PopPanel()
        {
            if (_panelStack.Count == 0) return;
            _panelStack.Pop().OnExit();
            
            if (_panelStack.Count == 0) return;
            _panelStack.Peek().OnResume();
        }
        
        public void ClearPanel()
        {
            while (_panelStack.Count > 0) _panelStack.Peek().OnExit();
        }
        
        #endregion
    }
}