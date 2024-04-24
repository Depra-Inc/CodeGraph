// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Reflection;

namespace Depra.CodeGraph.Editor.View
{
	internal static class MemberInfoExtensions
	{
		public static bool HasAttribute<T>(this MemberInfo self) where T : Attribute =>
			self.GetCustomAttribute<T>() != null;

		public static bool TryGetAttribute<T>(this MemberInfo self, out T attribute) where T : Attribute
		{
			attribute = self.GetCustomAttribute<T>();
			return attribute != null;
		}
	}
}