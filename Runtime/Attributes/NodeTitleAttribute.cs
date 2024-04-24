using System;

namespace Depra.CodeGraph.Attributes
{
	public sealed class NodeTitleAttribute : Attribute
	{
		public string Title { get; }

		public NodeTitleAttribute(string title) => Title = title;
	}
}