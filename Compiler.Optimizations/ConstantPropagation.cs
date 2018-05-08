using System.Collections.Generic;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.Optimizations
{
	public class ConstantPropagation : IOptimization
	{
		public List<Node> Optimize(List<Node> nodes, out bool applied)
		{
			var app = false;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i] is Assign node && node.Operation == OpCode.Copy && node.Right is IntConst)
				{
					for (int j = i + 1; j < nodes.Count; j++)
					{
						if (nodes[j] is Assign nextNode)
						{
							//Если мы встретили объявление этого же элемента
							if (node.Result.Equals(nextNode.Result))
								break;
							//Проверка использования Result в левом операнде другого узла
							if (node.Result.Equals(nextNode.Left))
							{
								nextNode.Left = node.Right;
								nodes[j] = nextNode;
								app = true;
							}
							//Проверка использования Result в правом операнде другого узла
							if (node.Result.Equals(nextNode.Right))
							{
								nextNode.Right = node.Right;
								nodes[j] = nextNode;
								app = true;
							}
						}
					}
				}
			}
			applied = app;
			return nodes;
		}
	}
}
