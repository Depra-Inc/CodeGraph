// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.CodeGraph.Connections;
using Depra.CodeGraph.Editor.Search;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.CodeGraph.Editor.View
{
	internal sealed class CodeGraphView : GraphView
	{
		private const string STYLE_SHEET_FILE = "CodeGraph";

		private readonly EditorWindow _window;
		private readonly CodeGraphNodeTree _tree;
		private readonly SerializedObject _serializedObject;
		private readonly CodeGraphSearchProvider _searchProvider;
		private readonly List<CodeGraphEditorNode> _nodes = new();
		private readonly Dictionary<Edge, CodeGraphConnection> _edgeLookup = new();
		private readonly Dictionary<string, CodeGraphEditorNode> _nodeLookup = new();

		public CodeGraphView(EditorWindow editorWindow, SerializedObject serializedObject)
		{
			_window = editorWindow;
			_serializedObject = serializedObject;
			_tree = (CodeGraphNodeTree) serializedObject.targetObject;
			_ = new CodeGraphViewListener(this, _serializedObject);

			_searchProvider = ScriptableObject.CreateInstance<CodeGraphSearchProvider>();
			_searchProvider.View = this;
			nodeCreationRequest = ShowSearchWindow;

			AddGridBackground();

			this.AddManipulator(new ClickSelector());
			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			AddStyles();
			PopulateNodes(_tree.Nodes);
			PopulateConnections(_tree.Connections);
			Bind();
		}

		internal Vector2 WindowPosition => _window.position.position;

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var allPorts = new List<Port>();
			foreach (var node in _nodes)
			{
				allPorts.AddRange(node.Ports);
			}

			var result = new List<Port>();
			foreach (var port in allPorts)
			{
				if (port == startPort || port.node == startPort.node || startPort.direction == port.direction)
				{
					continue;
				}

				if (startPort.portType == port.portType)
				{
					result.Add(port);
				}
			}

			return result;
		}

		public void Create(CodeGraphNode node)
		{
			Undo.RecordObject(_serializedObject.targetObject, "Added Node");

			_tree.Nodes.Add(node);
			_serializedObject.Update();

			AddNode(node);
			Bind();
		}

		private void PopulateNodes(IEnumerable<CodeGraphNode> graphNodes)
		{
			foreach (var node in graphNodes)
			{
				AddNode(node);
			}
		}

		private void PopulateConnections(IEnumerable<CodeGraphConnection> connections)
		{
			foreach (var connection in connections)
			{
				var inputNode = _nodeLookup.GetValueOrDefault(connection.InputPort.NodeId);
				var outputNode = _nodeLookup.GetValueOrDefault(connection.OutputPort.NodeId);
				if (inputNode == null || outputNode == null)
				{
					return;
				}

				var inPort = inputNode.Ports[connection.InputPort.Index];
				var outPort = outputNode.Ports[connection.OutputPort.Index];
				var edge = inPort.ConnectTo(outPort);
				AddElement(edge);

				_edgeLookup.Add(edge, connection);
			}
		}

		private void ShowSearchWindow(NodeCreationContext context)
		{
			_searchProvider.Target = (VisualElement) focusController.focusedElement;
			SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchProvider);
		}

		private void AddNode(CodeGraphNode node)
		{
			node.TypeName = node.GetType().AssemblyQualifiedName;

			var editorNode = new CodeGraphEditorNode(node, _serializedObject);
			editorNode.SetPosition(node.Position);
			_nodes.Add(editorNode);
			_nodeLookup.Add(node.Guid, editorNode);

			AddElement(editorNode);
		}

		internal void RemoveNode(CodeGraphEditorNode editorNode)
		{
			Undo.RecordObject(_serializedObject.targetObject, "Removed Node");

			_nodes.Remove(editorNode);
			_tree.Nodes.Remove(editorNode.Node);
			_nodeLookup.Remove(editorNode.Node.Guid);
			_serializedObject.Update();
		}

		internal void RemoveConnection(Edge edge)
		{
			if (_edgeLookup.Remove(edge, out var connection))
			{
				_tree.Connections.Remove(connection);
			}
		}

		private void AddGridBackground()
		{
			var grid = new GridBackground();
			grid.StretchToParentSize();
			grid.SendToBack();
			Insert(0, grid);
		}

		private void AddStyles()
		{
			var styleSheetGuid = AssetDatabase.FindAssets(STYLE_SHEET_FILE)[0];
			var assetPath = AssetDatabase.GUIDToAssetPath(styleSheetGuid);
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath);
			styleSheets.Add(styleSheet);
		}

		internal void CreateEdge(Edge edge)
		{
			var inputNode = (CodeGraphEditorNode) edge.input.node;
			var outputNode = (CodeGraphEditorNode) edge.output.node;

			var inputIndex = inputNode.Ports.IndexOf(edge.input);
			var outputIndex = outputNode.Ports.IndexOf(edge.output);

			var connection = new CodeGraphConnection(inputNode.Id, inputIndex, outputNode.Id, outputIndex);
			_tree.Connections.Add(connection);
		}

		private void Bind()
		{
			_serializedObject.Update();
			this.Bind(_serializedObject);
		}
	}
}