using UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSystem
{
    public class TacticScene : IScene
    {
        private const string SceneName = "TacticScene";
        public void Init()
        {
            var loadHandle = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            if (loadHandle != null)
                loadHandle.completed += operation =>
                {
                    UIManager.Instance.PushPanel(PanelType.TacticPanel, null);
                };
            else
            {
                Debug.LogError($"LoadSaveByName Scene Failed，请检查场景名字是否正确 {SceneName}");
            }
        }

        public void Release()
        {
            UIManager.Instance.ClearPanel();
        }
    }
}