using Utility;

namespace GamePlaySystem.DeploySystem
{
    public static class PlayerData
    {
        // TODO 创建玩家数据类 替换此实现
        public static readonly BindableProperty<int> Gold = new();
        public static int partySize = 0;
        public static int maxPartySize = 4;
    }
}