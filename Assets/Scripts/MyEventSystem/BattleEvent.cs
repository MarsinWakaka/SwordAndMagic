namespace MyEventSystem
{
    // [个人总结]使用枚举的好处(相比于字符串的优势)：
    // 即使是事件中心为单例模式，难以查找调用事件的地方
    // 但是通过枚举变量的[查找用例]可以很方便地查找到调用事件的地方。
    /// <summary>
    /// 这是战斗场景中的事件枚举，请确保退出场景时清空该类型的事件中心
    /// </summary>
    public enum GameEvent
    {
        // 游戏阶段切换
        GameResourceLoadStart,
        GameResourceLoadEnd,
        ScenarioStart,
        ScenarioEnd,
        PlayerDeployedStart,
        PlayerDeployedEnd,
        BattleStart,
        BattleEnd,
        
        // 部署阶段
        DeployCharacterResource,     // 部署资源
        DeployCharacterSelected,     // 部署角色
        DeployCharacterSuccess,      // 部署成功
        // 鼠标
        // OnLeftMouseClick,    // 鼠标左键点击
        OnRightMouseClick,      // 鼠标右键点击
        // 来自游戏实体的事件
        OnEntityLeftClicked,  // 实体被左键点击
        OnEntityRightClicked, // 实体被右键点击
        OnEnemyLeftClicked,  // 敌人被左键点击
        OnEnemyRightClicked, // 敌人被右键点击
        OnPlayerLeftClicked, // 玩家角色被左键点击
        OnPlayerRightClicked,// 玩家角色被右键点击
        OnTileLeftClicked,   // 地块被左键点击
        OnTileRightClicked,  // 地块被右键点击
        
        // UI通知角色管理器的事件
        OnCharacterSlotUIClicked, // 角色被选中时触发：UI点击来设置角色选中 -> 告知角色管理器
        OnSkillSlotUIClicked,        // 技能被选中时触发：UI点击来设置技能选中 -> 告知技能管理器
        OnSkillReleaseButtonClicked, // 技能释放按钮被点击 -> 告知技能管理器

        // 管理器通知UI的事件
        UpdateUIOfActionUnitOrder,  // 更新行动单位顺序
        UpdateUIOfPlayerParty,      // 管理器设置玩家队伍 -> 通知UI更新
        UpdateUIOfSelectedCharacter,// 管理器设置玩家角色回合开始 -> 通知UI更新
        OpenBattleEndPanel,         // 战斗结束 -> 打开战斗结束面板
        
        // 角色控制器通知技能UI的事件
        OnSkillTargetChoseCountChanged, // 技能目标选择数量变化 -> 告知UI更新
        OnSKillChosenStateEnter,        // 技能选择状态进入 -> 告知UI更新
        OnSKillChosenStateExit,         // 技能选择状态退出 -> 告知UI更新
        
        SetHoverEntity,      // 更新调查面板
        
        // 实体创建事件
        OnTileCreated,
        OnCharacterCreated,
        OnCharacterDeath, 
        
        ShowRangeOperation,          // 显示范围
        CloseRangeOperation,         // 关闭范围
        
        // 相机事件
        CameraMoveToPosition,        // 相机移动到指定位置
    }
}