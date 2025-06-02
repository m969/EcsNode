using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public interface IEventRun
    {
        EcsNode EcsNode { get; set; }
    }

    public abstract class AEventRun<A> : IEventRun where A : EcsEntity
    {
        public abstract EcsNode EcsNode { get; set; }
        protected abstract ETTask Run(A a);
        public async ETTask Handle(A a)
        {
            try
            {
                await Run(a);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }

    public abstract class AEventRun<A1, A2> : IEventRun where A1 : EcsEntity
    {
        public abstract EcsNode EcsNode { get; set; }
        protected abstract ETTask Run(A1 a1, A2 a2);
        public async ETTask Handle(A1 a1, A2 a2)
        {
            try
            {
                await Run(a1, a2);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }

    public abstract class AEventRun<A1, A2, A3> : IEventRun where A1 : EcsEntity
    {
        public abstract EcsNode EcsNode { get; set; }
        protected abstract ETTask Run(A1 a1, A2 a2, A3 a3);
        public async ETTask Handle(A1 a1, A2 a2, A3 a3)
        {
            try
            {
                await Run(a1, a2, a3);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }
}
