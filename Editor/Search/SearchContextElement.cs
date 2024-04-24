// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.CodeGraph.Editor.Search
{
	internal struct SearchContextElement
	{
		public SearchContextElement(object target, string title)
		{
			Target = target;
			Title = title;
		}

		public string Title { get; }
		public object Target { get; }
	}
}