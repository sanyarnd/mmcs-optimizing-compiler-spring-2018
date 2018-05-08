using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.Optimizations
{
	public class ConstantFolding : IOptimization
	{
		private bool SetNode(Assign node)
		{
			switch (node.Operation)
			{
				case OpCode.Plus:
					node.Right = (IntConst)node.Left + (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Minus:
					node.Right = (IntConst)node.Left - (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Mul:
					node.Right = (IntConst)node.Left * (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Div:
					node.Right = (IntConst)node.Left / (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return true;
		}
		public List<Node> Optimize(List<Node> nodes, out bool applied)
		{
			var app = false;
			var enumerable = nodes.OfType<Assign>().Where(assgn => assgn.Left is IntConst && assgn.Right is IntConst);

			foreach (var node in enumerable)
				app = SetNode(node);

			applied = app;
			return nodes;
		}
	}
}
