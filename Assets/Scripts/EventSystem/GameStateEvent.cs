namespace EventSystem
{
    // public static class CommonEvent
    // {
    //     public const string GameStateScenarioStart = "GameStateScenarioStart";
    //     public const string GameStateScenarioEnd = "GameStateScenarioEnd";
    //     public const string GameStatePlayerDeployedStart = "GameStatePlayerDeployedStart";
    //     public const string GameStatePlayerDeployedEnd = "GameStatePlayerDeployedEnd";
    //     public const string GameStateBattleStart = "GameStateBattleStart";
    //     public const string GameStateBattleEnd = "GameStateBattleEnd";
    // }
    
    public enum GameStateEvent
    {
        GameStateGameResourceLoadStart,
        GameStateGameResourceLoadEnd,
        GameStateScenarioStart,
        GameStateScenarioEnd,
        GameStatePlayerDeployedStart,
        GameStatePlayerDeployedEnd,
        GameStateBattleStart,
        GameStateBattleEnd
    }
}