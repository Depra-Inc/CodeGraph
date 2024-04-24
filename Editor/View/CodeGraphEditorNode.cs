// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Reflection;
using Depra.CodeGraph.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

namespace Depra.CodeGraph.Editor.View
{
	public sealed class CodeGraphEditorNode : Node
	{
		private readonly SerializedObject _serializedObject;

		private Port _outputPort;
		private SerializedProperty _serializedProperty;

		public CodeGraphEditorNode(CodeGraphNode node, SerializedObject serializedObject)
		{
			Node = node;
			_serializedObject = serializedObject;

			AddToClassList("code-graph-node");
			var type = Node.GetType();

			name = type.Name;
			if (type.TryGetAttribute<NodeTitleAttribute>(out var titleInfo))
			{
				title = titleInfo.Title;
			}

			foreach (var depth in type.GetCustomAttribute<NodeMenuPathAttribute>().Path.Split('/'))
			{
				AddToClassList(depth.ToLower().Replace(' ', '-'));
			}

			if (type.HasAttribute<HasFlowInputAttribute>())
			{
				CreateInputPort();
			}

			if (type.HasAttribute<HasFlowOutputAttribute>())
			{
				CreateOutputPort();
			}

			DrawProperties(type);
		}

		public string Id => Node.Guid;
		public CodeGraphNode Node { get; }
		public List<Port> Ports { get; } = new();
		public void SavePosition() => Node.Position = GetPosition();

		private void CreateInputPort()
		{
			var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
				Port.Capacity.Single, typeof(PortTypes.FlowPort));
			inputPort.portName = "In";
			inputPort.tooltip = "The flow input";
			Ports.Add(inputPort);

			inputContainer.Add(inputPort);
		}

		private void CreateOutputPort()
		{
			_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output,
				Port.Capacity.Single, typeof(PortTypes.FlowPort));
			_outputPort.portName = "Out";
			_outputPort.tooltip = "The flow output";
			Ports.Add(_outputPort);

			outputContainer.Add(_outputPort);
		}

		private void DrawProperties(Type type)
		{
			var properties = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var property in properties)
			{
				if (property.IsStatic)
				{
					continue;
				}

				if (property.IsPublic ||
				    property.HasAttribute<SerializeField>() ||
				    property.HasAttribute<SerializeReference>())
				{
					DrawPropertyField(property.Name).RegisterValueChangeCallback(OnFieldValueChanged);
				}
			}
		}

		private PropertyField DrawPropertyField(string propertyName)
		{
			_serializedProperty ??= FetchSerializedProperty();
			var property = _serializedProperty.FindPropertyRelative(propertyName);
			var field = new PropertyField(property) { bindingPath = property.propertyPath };
			extensionContainer.Add(field);
			RefreshExpandedState();

			return field;
		}

		private SerializedProperty FetchSerializedProperty()
		{
			var nodes = _serializedObject.FindProperty("_nodes");
			if (nodes.isArray == false)
			{
				return null;
			}

			var size = nodes.arraySize;
			for (var index = 0; index < size; index++)
			{
				var element = nodes.GetArrayElementAtIndex(index);
				var elementId = element.FindPropertyRelative("_guid").stringValue;
				if (elementId == Node.Guid)
				{
					return element;
				}
			}

			return null;
		}

		private void OnFieldValueChanged(SerializedPropertyChangeEvent evt) { }
	}
}