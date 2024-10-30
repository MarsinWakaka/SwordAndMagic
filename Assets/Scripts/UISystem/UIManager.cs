using System;
using System.Collections.Generic;
using ConsoleSystem;
using ResourcesSystem;
using UnityEngine;
using Utility.Singleton;

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

    public sealed class UIManager : SingletonMono<UIManager>
    {
        private static string UIPanelPath => ApplicationRoot.Instance.Config.uiPanelPath;
        // [SerializeField] AssetReference mainMenusPanelRef;
        [SerializeField] private Transform panelRoot;
        private readonly Dictionary<PanelType, BasePanel> _panelDict = new ();
        private readonly Stack<BasePanel> _panelStack = new ();

        /// <summary>
        /// 异步方法加载Panel
        /// </summary>
        private void LoadPanelAsync(PanelType panelType, Action<BasePanel> onComplete)
        {
            var resURL = $"{UIPanelPath}/{panelType}.prefab";
            ServiceLocator.Get<IResourceManager>().LoadResourceAsync<GameObject>(resURL, (go) =>
            {
                if (go == null) return;
                var panelComponent = go.GetComponent<BasePanel>();
                if (panelComponent)
                {
                    var panel = Instantiate(panelComponent, panelRoot);
                    _panelDict.Add(panelType, panel);
                    onComplete?.Invoke(panel);
                }
                else
                {
                    Debug.LogError($"加载失败: {resURL}");
                }
            });
        }
        
        private void GetPanel(PanelType panelType, Action<BasePanel> onComplete)
        {
            if (_panelDict.TryGetValue(panelType, out var panel)) {
                onComplete?.Invoke(panel);
                return;
            }
            LoadPanelAsync(panelType, onComplete);
        }
        
        
        #region 功能区
        
        public BasePanel GetCurrentPanel() => _panelStack.Peek();
        
        public void PushPanel(PanelType panelType, Action onComplete)
        {
            MyConsole.Print($"PushPanel {panelType}");
            if (_panelStack.Count > 0) _panelStack.Peek().OnPause();
            GetPanel(panelType, (panel) =>
            {
                _panelStack.Push(panel);
                panel.OnEnter();
                onComplete?.Invoke();
            });
        }
        
        public void PopPanel()
        {
            MyConsole.Print($"PopPanel {_panelStack.Peek()}");
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