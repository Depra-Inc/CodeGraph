// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Depra.CodeGraph.Editor.View
{
	internal sealed class CodeGraphViewListener : IDisposable
	{
		private readonly CodeGraphView _graph;
		private readonly SerializedObject _serializedObject;

		public CodeGraphViewListener(CodeGraphView graph, SerializedObject serializedObject)
		{
			_graph = graph;
			_serializedObject = serializedObject;
			_graph.graphViewChanged += OnGraphViewChanged;
		}

		public void Dispose()
		{
			_graph.graphViewChanged -= OnGraphViewChanged;
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange change)
		{
			if (change.movedElements != null)
			{
				ProcessMovedNodes(change);
			}

			if (change.elementsToRemove != null)
			{
				ProcessDeletedNodes(change);
				ProcessDeletedConnections(change);
			}

			if (change.edgesToCreate != null)
			{
				ProcessEdges(change);
			}

			return change;
		}

		private void ProcessMovedNodes(GraphViewChange change)
		{
			Undo.RecordObject(_serializedObject.targetObject, "Moved Elements");

			foreach (var node in change.movedElements.OfType<CodeGraphEditorNode>())
			{
				node.SavePosition();
			}
		}

		private void ProcessDeletedNodes(GraphViewChange change)
		{
			var editorNodes = change.elementsToRemove.OfType<CodeGraphEditorNode>().ToList();
			if (editorNodes.Count <= 0)
			{
				return;
			}

			Undo.RecordObject(_serializedObject.targetObject, "Removed Elements");

			for (var index = editorNodes.Count - 1; index >= 0; index--)
			{
				_graph.RemoveNode(editorNodes[index]);
			}
		}

		private void ProcessDeletedConnections(GraphViewChange change)
		{
			foreach (var edge in change.elementsToRemove.OfType<Edge>())
			{
				_graph.RemoveConnection(edge);
			}
		}

		private void ProcessEdges(GraphViewChange change)
		{
			Undo.RecordObject(_serializedObject.targetObject, "Added Connections");

			foreach (var edge in change.edgesToCreate)
			{
				_graph.CreateEdge(edge);
			}
		}
	}
}