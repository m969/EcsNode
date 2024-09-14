using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
	public interface IPreInit
	{
	}

	public interface IInit
	{
	}

	public interface IPostInit
	{
	}

	public interface IInit<T, T2> : IInit where T : EcsEntity where T2 : IEcsComponent
	{
		void Init(T entity, T2 component);
	}
}