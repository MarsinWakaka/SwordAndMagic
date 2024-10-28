namespace Entity.Unit
{
    /// <summary>
    /// 战旗状态类型
    /// </summary>
    public enum CharacterStateType
    {
        // TODO 考虑将下面转为角色的状态，类似进程状态一样
        Inactive,           // 角色非激活状态
        Active,             // 角色激活状态
        OnDead,             // 角色死亡
        
        // TODO 考虑将下面转为角色输入控制器的状态
        Disabled,           // 角色控制器不可操作状态
        // 以下为角色[激活]且[被选中操作时]的状态
        WaitForCommand,     // 角色控制器等待玩家操作状态
        Moving,           // 角色移动中
        OnSkillChosen,      // 角色选择了技能时的状态
        OnSkillRelease,    // 角色释放技能时的状态
    }
}