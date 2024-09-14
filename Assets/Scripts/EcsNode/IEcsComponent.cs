
namespace ECS
{
    /// <summary>
    /// 组件是分离组合的机制，各个组件平级且分离
    /// </summary>
    public interface IEcsComponent
    {
        long Id { get; set; }
    }
}