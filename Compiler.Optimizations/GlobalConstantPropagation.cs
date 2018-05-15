using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.DFA;
using Compiler.ThreeAddrCode.DFA.ConstantPropagation;

namespace Compiler.Optimizations
{
	public class GlobalConstantPropagation
	{
		public InOutData<Dictionary<Guid, VarValue>> TempFunc(TACode taCode, ControlFlowGraph cfg)
		{
			Operations ops = new Operations(taCode);
			TransferFunctions f = new TransferFunctions();

			IterativeAlgorithm itAlg = new IterativeAlgorithm();
			var result = itAlg.Analyze(cfg, ops, f);

			return result;
		}
		public TACode Optimize(TACode taCode, out bool applied)
		{
			var app = false;
			var visited = new Dictionary<Guid, bool>();
			ControlFlowGraph cfg = new ControlFlowGraph(taCode);
			var ioData = TempFunc(taCode, cfg);

			foreach (var node in taCode.CodeList.ToList().OfType<Assign>())
				visited[node.Result.Id] = false;

			for (int j = taCode.CodeList.Count() - 1; j > 0; j--)
			{
				var node = taCode.CodeList.ElementAt(j) as Assign;
				if (node != null)
				{
					for (int i = 0; i < cfg.CFGNodes.Count(); i++)
					{
						if (ioData[cfg.CFGNodes.ElementAt(i)].Item1.ContainsKey(node.Result.Id) && ioData[cfg.CFGNodes.ElementAt(i)].Item1[node.Result.Id].varType is VarValue.Type.CONST)
						{
							if (visited[node.Result.Id] == true)
								break;
							visited[node.Result.Id] = true;
							node.Right = ioData[cfg.CFGNodes.ElementAt(i)].Item1[node.Result.Id].value;
							node.Left = null;
							node.Operation = OpCode.Copy;
						}
					}
				}
			}

			applied = app;
			return taCode;
		}
	}
}
