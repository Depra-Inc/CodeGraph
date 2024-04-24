﻿// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.CodeGraph.Attributes
{
	public sealed class NodeMenuPathAttribute : Attribute
	{
		public string Path { get; }

		public NodeMenuPathAttribute(string path) => Path = path;
	}
}