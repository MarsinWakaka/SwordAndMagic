# 项目介绍

***

### 背景

目前该战旗游戏由之前的一个战旗Demo迭代过来，项目保留了原来的代码(Obsolete命名空间下)。
该项目仍处于开发中，且部分目前为了测试方便游戏部分功能进行了自动化处理。

***

### 项目实践介绍。

- 使用事件中心来负责不同模块间的通信，实现不同模块间的解耦。
- 使用自定义关卡文件格式，关卡管理器通过编写的解析器从中磁盘中读取关卡数据并交由实体工厂生成。
- 使用状态模式来制作角色控制器，控制选中角色的行为逻辑切换（闲置，移动，技能选择，技能释放等)
- 制作了技能框架，只需要重写核心的执行函数即可添加新的技能类型，其它例如技能范围，最大目标个数等等都能通过简单调整参数来实现技能变种，技能参数类通过继承ScriptableObject来配置。
- 实现数据结构BindableProperty<T>，使得任何类型都可以被监听变化，常用于实现数据与UI的同步。
- 基于BFS的移动范围显示，基于DDA画线算法的角色视野系统(攻击范围显示)。
