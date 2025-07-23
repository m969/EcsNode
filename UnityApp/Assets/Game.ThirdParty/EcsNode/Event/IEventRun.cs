using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public interface IDomainEvent
    {
    }

    public interface IEventRun
    {
        public ETTask Handle(EcsNode domain, IDomainEvent a);
    }

    public abstract class AEventRun<T, A> : IEventRun where T : EcsNode where A : IDomainEvent
    {
        protected abstract ETTask Run(T domain, A a);
        public async ETTask Handle(EcsNode domain, IDomainEvent a)
        {
            try
            {
                await Run((T)domain, (A)a);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }

    //public abstract class AEventRun<A1, A2> : IEventRun where A1 : EcsEntity
    //{
    //    protected abstract ETTask Run(A1 a1, A2 a2);
    //    public async ETTask Handle(A1 a1, A2 a2)
    //    {
    //        try
    //        {
    //            await Run(a1, a2);
    //        }
    //        catch (Exception e)
    //        {
    //            ConsoleLog.Error(e);
    //        }
    //    }
    //}

    //public abstract class AEventRun<A1, A2, A3> : IEventRun where A1 : EcsEntity
    //{
    //    protected abstract ETTask Run(A1 a1, A2 a2, A3 a3);
    //    public async ETTask Handle(A1 a1, A2 a2, A3 a3)
    //    {
    //        try
    //        {
    //            await Run(a1, a2, a3);
    //        }
    //        catch (Exception e)
    //        {
    //            ConsoleLog.Error(e);
    //        }
    //    }
    //}
}
