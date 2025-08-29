using ECS;

namespace ECSGame.Module.Player
{
    /// <summary>
    /// 表示游戏中的玩家对象（当前版本假定单一本地玩家），承载生命周期状态并可被系统管理。
    /// 仅承载必要属性数据；不实现任何方法逻辑。
    /// </summary>
    public partial class Player : EcsEntity
    {
        // 按项目规范，实体只包含属性数据；当前无额外属性。
    }
}
