// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.CodeGraph.Editor
{
	[CustomEditor(typeof(RuntimeCodeGraph))]
	internal sealed class CodeGraphEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var sceneEntryPoint = (RuntimeCodeGraph) target;
			if (sceneEntryPoint.NodeTree == null)
			{
				return;
			}

			if (GUILayout.Button("Open"))
			{
				CodeGraphEditorWindow.Open(sceneEntryPoint.NodeTree);
			}
		}
	}
}