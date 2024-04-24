// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.CodeGraph.Connections
{
	[Serializable]
	public struct CodeGraphConnection
	{
		public CodeGraphConnectionPort InputPort;
		public CodeGraphConnectionPort OutputPort;

		public CodeGraphConnection(CodeGraphConnectionPort inputPort, CodeGraphConnectionPort outputPort)
		{
			InputPort = inputPort;
			OutputPort = outputPort;
		}

		public CodeGraphConnection(string inputPortId, int inputIndex, string outputPortId, int outputIndex)
		{
			InputPort = new CodeGraphConnectionPort(inputPortId, inputIndex);
			OutputPort = new CodeGraphConnectionPort(outputPortId, outputIndex);
		}
	}
}