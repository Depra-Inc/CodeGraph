// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.CodeGraph.Editor.View;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Depra.CodeGraph.Editor
{
	internal sealed class CodeGraphEditorWindow : EditorWindow
	{
		public static void Open(CodeGraphNodeTree target) =>
			GetWindow<CodeGraphEditorWindow>(target.name).Load(target);

		private CodeGraphView _currentView;
		private CodeGraphNodeTree _currentNodeTree;
		private SerializedObject _serializedObject;

		private void OnEnable()
		{
			if (_currentNodeTree != null)
			{
				DrawGraph();
			}
		}

		private void OnGUI()
		{
			if (_currentNodeTree != null)
			{
				hasUnsavedChanges = EditorUtility.IsDirty(_currentNodeTree);
			}
		}

		private void Load(CodeGraphNodeTree target)
		{
			_currentNodeTree = target;
			DrawGraph();
		}

		private void DrawGraph()
		{
			_serializedObject = new SerializedObject(_currentNodeTree);

			_currentView = new CodeGraphView(this, _serializedObject);
			_currentView.graphViewChanged += OnViewChanged;
			_currentView.StretchToParentSize();

			rootVisualElement.Add(_currentView);
		}

		private GraphViewChange OnViewChanged(GraphViewChange change)
		{
			EditorUtility.SetDirty(_currentNodeTree);
			return change;
		}
	}
}