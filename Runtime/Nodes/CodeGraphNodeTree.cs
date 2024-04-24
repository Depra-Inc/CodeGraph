// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using Depra.CodeGraph.Connections;
using UnityEngine;
using static Depra.CodeGraph.Module;

namespace Depra.CodeGraph
{
	[CreateAssetMenu(menuName = MENU_PATH + FILE_NAME, fileName = FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class CodeGraphNodeTree : ScriptableObject
	{
		[SerializeReference] private List<CodeGraphNode> _nodes;
		[SerializeField] private List<CodeGraphConnection> _connections;

		private const string FILE_NAME = nameof(CodeGraphNodeTree);

		internal List<CodeGraphNode> Nodes => _nodes;
		internal List<CodeGraphConnection> Connections => _connections;

		internal StartNode GetStartNode() => Nodes.OfType<StartNode>().First();

		internal CodeGraphNode GetNode(string guid) => Nodes.FirstOrDefault(node => node.Guid == guid);

		internal CodeGraphNode GetNodeFromOutput(string outputNode) => (from connection in Connections
			where connection.OutputPort.NodeId == outputNode
			select GetNode(connection.InputPort.NodeId)).FirstOrDefault();
	}
}