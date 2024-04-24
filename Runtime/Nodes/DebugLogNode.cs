// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.CodeGraph.Attributes;
using UnityEngine;

namespace Depra.CodeGraph
{
	[HasFlowInput]
	[HasFlowOutput]
	[NodeTitle("Debug Log")]
	[NodeMenuPath("Debug/Debug Log Console")]
	public sealed class DebugLogNode : CodeGraphNode
	{
		[SerializeField] private string _message;

		protected override void Execute(Context context) => Debug.Log(_message);
	}
}