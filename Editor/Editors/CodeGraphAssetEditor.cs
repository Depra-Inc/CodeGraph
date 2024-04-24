// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Depra.CodeGraph.Editor
{
	[CustomEditor(typeof(CodeGraphNodeTree))]
	internal sealed class CodeGraphAssetEditor : UnityEditor.Editor
	{
		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int index)
		{
			var asset = EditorUtility.InstanceIDToObject(instanceId);
			if (asset is not CodeGraphNodeTree codeGraphAsset)
			{
				return false;
			}

			CodeGraphEditorWindow.Open(codeGraphAsset);
			return true;
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open"))
			{
				CodeGraphEditorWindow.Open((CodeGraphNodeTree) target);
			}
		}
	}
}