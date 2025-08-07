# EcsNode

基于Entity-Component-System的框架

支持热重载

# 游戏高效编程范式理念

- 数据逻辑分离（Component-System）

- 业务渲染分离（System-ViewSystem）

- 组合优于继承（Entity-Component）

- 领域驱动设计（EcsNode，领域可看做不同的业务场景，规划对齐需求粒度，降低协作理解成本）

- 高内聚低耦合（高内聚优先于低耦合，浅传参优先于深传参(即扁平化逻辑)）

- 面向切面编程（避免侵入式编码）

- 规范式模块化（Spec-Modular-Driven）AI代码生成流程（类似于Kiro的Spec-Driven，基于copilot-instruction.md）
    - 常用prompt示例：
        - 补充完善require需求文档
        - 根据require文档，更新完善design文档
        - 根据design文档，更新完善program文档
        - 根据program文档，更新完善program-test文档
        - 根据program文档，更新完善配置接口、实体、组件、类型补充代码文件
        - 根据program文档，更新完善配置接口代码文件
        - 根据program文档，更新完善实体代码文件
        - 根据program文档，更新完善组件代码文件
        - 根据program文档，更新完善系统代码文件
        - 根据program-test文档和模块System代码，更新完善单元测试代码文件

示例:
- Modules.Unity/com.module.resource-data（资源模块）
- Modules.Unity/com.module.achieve（达成模块，用于成就和任务等）

copilot-instruction.md 里的都是自然语言描述的指导文档，亦可用于别的大模型指导文档，比如CLAUDE.md、cursor rule等

<img src="Readme/folder-info.png" width="35%">
<img src="Readme/folder-info2.png" width="40%">

<img src="Readme/modules-info.png" width="40%">

# SLG游戏demo（开发中）

- Modules.Unity/com.module.resource-data（资源模块）
- Modules.Unity/com.module.grid-based（网格系统）
- Modules.Unity/com.module.building（建筑建造模块）
- Modules.Unity/com.module.achieve（达成模块，用于成就和任务等）

# 帧同步demo（开发中）

方案一：仅预测移动，冲突即重置回滚重新预测（已实现）

方案二：预测碰撞事件缓存，先做特效表现，等待权威帧验证，验证通过则继续走逻辑，验证不通过则碰撞事件丢弃（未实现）

方案三：全部预测，缓存状态帧快照用以回滚（未实现）

- 帧同步demo，黄色物体是确定帧轨迹

<img src="Readme/帧同步demo.gif" width="100%">


## 基于实体和组件的数据驱动
model
entity

## 面向系统和过程的业务开发
system
1vN

数据分散（把数据分散到多个组件，降低运行时内存负担）
逻辑收束（将逻辑归纳总结为统一系统，降低开发者理解负担）

将定义和理解一致

强调系统的归属

与C#传统写法一致

框架里有两种事件机制，一种是模块间接交互事件，一种是System接口事件

间接交互事件用于间接调用，用于虽然能直接调用到但不应该直接调用的时候，属于间接解耦

System接口事件用于动态派发，用于不应该直接调用也调用不到的时候，属于弱解耦，比如业务层触发视图层逻辑的时候

完全解耦的事件机制，比如字符串事件，这种过于灵活，并不推荐使用，会增加代码的维护和理解成本

模块间接交互事件是显示调用的，非动态派发，用于同一层级的模块之间交互，比如业务层级模块之间的交互（比如FireEvent、CollisionEvent），视图层级模块之间的交互（比如InputEvent）

System接口事件

框架通用System接口（比如IAwake、IDestroy），所有实体组件都适用，调用时无法明确接口参数，需要通过反射实现，需要注册（EcsNode.RegisterDrive）

业务指定System接口（比如IAfterRunEvent），适用于指定实体，调用时明确知道接口参数，可以直接通过接口调用，无需注册

## 提升开发效率的核心在于降低测试的时间和成本，提升测试的效率
首先是核心玩法逻辑，每次改逻辑或者加日志都需要重开游戏就非常浪费时间

这个可以通过逻辑热重载降低测试成本

再一个就是UI开发，UI的需求变动非常频繁，每次改ui逻辑和界面都需要重开游戏非常浪费时间

这个可以结合FGUI和代码生成实现界面和代码热更新降低测试成本

ui框架不需要使用ecs，因为ui界面天生就有主次之分，不需要通过实体和组件来区分主次

比如window就是主，button、text、image等都是包含在window之内的组件，主次一目了然

ui框架直接基于面向对象来实现

只有业务功能框架，一般主次区分不明显，需要用到ecs的实体和组件来区分主次，方便理解

过程是逻辑的梳理和复用


# 参考
- https://github.com/egametang/ET

- https://github.com/Leopotam/ecslite

- https://github.com/vovgou/loxodon-framework

- https://github.com/liyingsong99/FolderTag