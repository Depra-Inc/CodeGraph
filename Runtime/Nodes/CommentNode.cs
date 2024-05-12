using System;
using Depra.CodeGraph.Attributes;
using UnityEngine;

namespace Depra.CodeGraph
{
	[Serializable]
	[NodeTitle("Comment")]
	[NodeMenuPath("Comment")]
	internal sealed class CommentNode : CodeGraphNode
	{
		[SerializeField] private string _text;
	}
}