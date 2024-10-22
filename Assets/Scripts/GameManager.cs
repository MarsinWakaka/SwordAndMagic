using BattleSystem.Emporium;
using Utility.Singleton;

namespace DefaultNamespace
{
    public class GameManager : SingletonMono<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            PlayerData.Gold.Value = 50;
        }
    }
}