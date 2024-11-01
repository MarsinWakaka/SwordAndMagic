namespace SceneSystem
{
    public interface IScene
    {
        public void Init();
        public void Release();
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameSceneManager
    {
        private static IScene _currentScene;

        public static void LoadScene(IScene scene)
        {
            _currentScene?.Release();
            _currentScene = scene;
            _currentScene.Init();
        }
    }
}