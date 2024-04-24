// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Depra.CodeGraph.Attributes;
using Depra.CodeGraph.Editor.View;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.CodeGraph.Editor.Search
{
	internal sealed class CodeGraphSearchProvider : ScriptableObject, ISearchWindowProvider
	{
		private static IEnumerable<SearchContextElement> CollectElements() =>
			from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			where type.CustomAttributes.Any()
			let attribute = type.GetCustomAttribute(typeof(NodeMenuPathAttribute))
			where attribute != null
			let info = (NodeMenuPathAttribute) attribute
			let node = Activator.CreateInstance(type)
			where !string.IsNullOrEmpty(info.Path)
			select new SearchContextElement(node, info.Path);

		private static List<SearchTreeEntry> PopulateTree(IEnumerable<SearchContextElement> elements)
		{
			var groups = new List<string>();
			var tree = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Nodes"))
			};

			foreach (var element in elements)
			{
				var entryTitle = element.Title.Split('/');
				var groupName = string.Empty;

				for (var index = 0; index < entryTitle.Length - 1; index++)
				{
					groupName += entryTitle[index];
					if (groups.Contains(groupName) == false)
					{
						tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[index]), index + 1));
						groups.Add(groupName);
					}

					groupName += "/";
				}

				var entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
				{
					level = entryTitle.Length,
					userData = new SearchContextElement(element.Target, element.Title)
				};
				tree.Add(entry);
			}

			return tree;
		}

		public CodeGraphView View;
		public VisualElement Target;

		List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
		{
			var elements = CollectElements().ToList();
			elements.Sort(new SearchContextElementComparer());
			var tree = PopulateTree(elements);

			return tree;
		}

		bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			var desiredPosition = context.screenMousePosition - View.WindowPosition;
			var windowMousePosition = View.ChangeCoordinatesTo(View, desiredPosition);
			var graphMousePosition = View.contentViewContainer.WorldToLocal(windowMousePosition);

			var element = (SearchContextElement) searchTreeEntry.userData;
			var node = (CodeGraphNode) element.Target;
			node.Position = new Rect(graphMousePosition, Vector2.zero);
			View.Create(node);

			return true;
		}

		private readonly struct SearchContextElementComparer : IComparer<SearchContextElement>
		{
			int IComparer<SearchContextElement>.Compare(SearchContextElement x, SearchContextElement y)
			{
				var splits1 = x.Title.Split('/');
				var splits2 = y.Title.Split('/');

				for (var index = 0; index < splits1.Length; index++)
				{
					if (index >= splits2.Length)
					{
						return 1;
					}

					if (string.Compare(splits1[index], splits2[index], StringComparison.Ordinal) == 0)
					{
						continue;
					}

					if (splits1.Length != splits2.Length && (index == splits1.Length - 1 || index == splits2.Length - 1))
					{
						return splits1.Length < splits2.Length ? 1 : -1;
					}
				}

				return 0;
			}
		}
	}
}