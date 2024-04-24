// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.CodeGraph.Attributes;

namespace Depra.CodeGraph
{
	[HasFlowOutput]
	[NodeTitle("Start")]
	[NodeMenuPath("Process/Start")]
	internal sealed class StartNode : CodeGraphNode { }
}