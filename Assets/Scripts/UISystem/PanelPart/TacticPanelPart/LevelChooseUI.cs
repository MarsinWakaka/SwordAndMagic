using System;
using System.Collections.Generic;
using Configuration;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UISystem.PanelPart.TacticPanelPart
{
    public class LevelChooseUI : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private LevelSlotUI levelSlotPrefab;
        [SerializeField] private Button backButton;
        private readonly List<LevelSlotUI> levelSlotList = new();
        private IObjectPool<LevelSlotUI> levelSlotPool;
        
        public Action OnCancelButtonClick;

        private void Awake()
        {
            levelSlotPool = new ObjectPool<LevelSlotUI>(
                ActionOnCreate,
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy,
                true, 10, 50);
            backButton.onClick.AddListener(() =>
            {
                OnCancelButtonClick?.Invoke();
            });
        }

        private void OnEnable()
        {
            // 获取关卡个数
            var count = ServiceLocator.Get<IConfigService>().ConfigData.levelCount;
            var slotCount = levelSlotList.Count;
            if (slotCount < count)
            {
                for (var i = slotCount; i < count; i++)
                {
                    levelSlotPool.Get();
                }
            }
            else if (slotCount > count)
            {
                for (var i = count - 1; i > slotCount; i--)
                {
                    levelSlotPool.Release(levelSlotList[i]);
                }
            }
        }
        
        private LevelSlotUI ActionOnCreate()
        {
            var slotUI = Instantiate(levelSlotPrefab, content);
            levelSlotList.Add(slotUI);
            slotUI.Initialize(levelSlotList.Count);  // 关卡索引从1开始
            return slotUI;
        }

        private void ActionOnGet(LevelSlotUI slotUI)
        {
            slotUI.gameObject.SetActive(true);
        }
        
        private void ActionOnRelease(LevelSlotUI slotUI)
        {
            slotUI.gameObject.SetActive(false);
        }
        
        private void ActionOnDestroy(LevelSlotUI slotUI)
        {
            Destroy(slotUI.gameObject);
        }
    }
}