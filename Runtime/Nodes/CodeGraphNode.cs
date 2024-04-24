// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.CodeGraph
{
	[Serializable]
	public class CodeGraphNode
	{
		[SerializeField] private string _guid = System.Guid.NewGuid().ToString();
		[field: SerializeField] public Rect Position { get; internal set; }
		[field: SerializeField] public string TypeName { get; internal set; }

		public string Guid => _guid;

		public string MoveNext(Context context)
		{
			Execute(context);

			var tree = context.Resolve<CodeGraphNodeTree>();
			var nextNode = tree.GetNodeFromOutput(Guid);

			return nextNode != null ? nextNode.Guid : string.Empty;
		}

		protected virtual void Execute(Context context) { }
	}
}