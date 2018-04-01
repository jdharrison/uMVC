﻿using System;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace Fuse.Implementation
{
	/// <summary>
	/// Invokes the method into the <see cref="ThreadPool"/>.
	/// WARNING: This is advanced, make sure what you are doing is thread-safe (Unity generally is not).
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method)]
	public class ThreadAttribute : Attribute, IFuseExecutor
	{
		public uint Order
		{
			get;
			[UsedImplicitly]
			private set;
		}

		public Lifecycle Lifecycle
		{
			get;
			[UsedImplicitly]
			private set;
		}

		public void Execute(MemberInfo target, object instance)
		{
			ThreadPool.QueueUserWorkItem(context => { ((MethodInfo) target).Invoke(instance, null); });
		}
	}
}