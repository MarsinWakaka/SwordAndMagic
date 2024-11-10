using System;
using System.Collections.Generic;
using Configuration;
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
        StartPanel,
        TacticPanel,
        CharacterEmporiumPanel,
        BattlePanel,
        BattleEndPanel,
        SavePanel,
        StartNewJourneyPanel,
        SettingPanel
    }

    public sealed class UIManager : SingletonMono<UIManager>
    {
        private string uiPanelPath;
        private string UIPanelPath {
            get {
                if (string.IsNullOrEmpty(uiPanelPath)) {
                    uiPanelPath = ServiceLocator.Get<IConfigService>().ConfigData.uiPanelPath;
                }
                return uiPanelPath;
            }
        }
        [SerializeField] private Transform panelRoot;
        private readonly Dictionary<PanelType, BasePanel> _panelDict = new ();
        private readonly Stack<BasePanel> _panelStack = new ();

        /// <summary>
        /// 异步方法加载Panel
        /// </summary>
        private void LoadPanelAsync(PanelType panelType, Action<BasePanel> onLoadComplete)
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
                    onLoadComplete?.Invoke(panel);
                }
                else
                {
                    Debug.LogError($"加载失败: {resURL}");
                }
            });
        }
        
        private void GetPanel(PanelType panelType, Action<BasePanel> onGetComplete)
        {
            if (_panelDict.TryGetValue(panelType, out var panel)) {
                onGetComplete?.Invoke(panel);
                return;
            }
            LoadPanelAsync(panelType, onGetComplete);
        }
        
        
        #region 功能区
        
        public BasePanel GetCurrentPanel() => _panelStack.Peek();
        
        public void PushPanel(PanelType panelType, Action onPushComplete)
        {
            MyConsole.Print($"PushPanel {panelType}");
            if (_panelStack.Count > 0) _panelStack.Peek().OnPause();
            GetPanel(panelType, (panel) =>
            {
                _panelStack.Push(panel);
                // 将其放在最上层
                panel.transform.SetAsLastSibling();
                panel.OnEnter();
                onPushComplete?.Invoke();
            });
        }
        
        [Obsolete("Use PopPanel(PanelType panelType) instead is recommended")]
        public void PopPanel()
        {
            MyConsole.Print($"PopPanel {_panelStack.Peek()}");
            if (_panelStack.Count == 0) return;
            _panelStack.Pop().OnExit();
            if (_panelStack.Count == 0) return;
            _panelStack.Peek().OnResume();
        }

        private const string ErrorMsgSuffix = ", please check the order of pop and push operation";
        /// <summary>
        /// 加入了类型检查，更加安全，弹出类型不匹配的面板时将会打印错误信息
        /// </summary>
        /// <param name="panelType"></param>
        public void PopPanel(PanelType panelType)
        {
            if (_panelStack.Count == 0) return;
            if (_panelStack.Peek().panelType == panelType)
            {
                if (_panelStack.Count == 0) return;
                _panelStack.Pop().OnExit();
                if (_panelStack.Count == 0) return;
                _panelStack.Peek().OnResume();
            } else {
                Debug.LogError(
                    $"current type is {_panelStack.Peek().panelType}, not match with the type of {panelType}{ErrorMsgSuffix}");
            }
        }
        
        public void ClearPanel()
        {
            while (_panelStack.Count > 0) _panelStack.Pop().OnExit();
        }
        
        #endregion
    }
}