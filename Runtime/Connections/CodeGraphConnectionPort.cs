// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.CodeGraph.Connections
{
	[Serializable]
	public struct CodeGraphConnectionPort
	{
		public int Index;
		public string NodeId;

		public CodeGraphConnectionPort(string nodeId, int index)
		{
			Index = index;
			NodeId = nodeId;
		}
	}
}