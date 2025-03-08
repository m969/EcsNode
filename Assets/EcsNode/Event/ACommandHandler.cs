using System;
using ET;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {

    }

    public interface ICommandHandler
    {
        Type Type { get; }
        public ETTask HandleCmd(ICommand cmd);
    }

    public abstract class ACommandHandler<T> : ICommandHandler where T : ICommand
    {
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        protected abstract ETTask Handle(T cmd);

        public async ETTask HandleCmd(ICommand cmd)
        {
            try
            {
                await Handle((T)cmd);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}