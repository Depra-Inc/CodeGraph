// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.CodeGraph
{
	public sealed class RuntimeCodeGraph : MonoBehaviour
	{
		[field: SerializeField] internal CodeGraphNodeTree NodeTree { get; private set; }

		private void OnEnable() => StartLoop(NodeTree.GetStartNode());

		private void StartLoop(CodeGraphNode currentNode)
		{
			var context = new Context();
			context.Register(NodeTree);

			while (true)
			{
				var nextNodeId = currentNode.MoveNext(context);
				if (string.IsNullOrEmpty(nextNodeId))
				{
					return;
				}

				var nextNode = NodeTree.GetNode(nextNodeId);
				currentNode = nextNode;
			}
		}
	}
}