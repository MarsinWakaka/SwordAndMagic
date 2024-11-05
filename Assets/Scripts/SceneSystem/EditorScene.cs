using UnityEngine.SceneManagement;

namespace SceneSystem
{
    public class EditorScene : IScene
    {
        private readonly string sceneName = "EditorScene";
        public void Init()
        {
            SceneManager.LoadScene(sceneName);
        }

        public void Release()
        {
            
        }
    }
}