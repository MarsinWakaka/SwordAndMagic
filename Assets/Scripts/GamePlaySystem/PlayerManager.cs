using Data;
using SaveSystem;
using Utility.Singleton;

namespace GamePlaySystem
{
    public class PlayerManager : SingletonMono<PlayerManager>, ISavable
    {
        public PlayerData playerData;

        public void Initialize()
        {
            var userService = ServiceLocator.Get<IUserSaveService>();
            userService.AddSaveSubscribe(this);
        }

        public void OnSave(ref UserData userData)
        {
            userData.playerData = playerData;
        }

        public void OnLoad(in UserData userData)
        {
            playerData = userData.playerData;
        }
    }
}