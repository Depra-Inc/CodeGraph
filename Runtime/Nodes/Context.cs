// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;

namespace Depra.CodeGraph
{
	public sealed class Context
	{
		private readonly Dictionary<object, object> _lookup = new();

		public object Resolve(object key) => _lookup[key];

		public void Register(object key, object value) => _lookup.Add(key, value);
	}

	public static class ContextExtensions
	{
		public static T Resolve<T>(this Context context) => (T) context.Resolve(typeof(T));

		public static void Register<T>(this Context context, T value) => context.Register(typeof(T), value);
	}
}