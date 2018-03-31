using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Optimizations;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Optimizations
{
	public class AllOptimizations
	{
		private List<IOptimization> CreateOptimizationList()
		{
			List<IOptimization> optimizations = new List<IOptimization>();

			AlgebraicOptimization algOptimization = new AlgebraicOptimization();
			CopyPropagation copyPropOptimization = new CopyPropagation();
			ConstantFolding constFoldingOptimization = new ConstantFolding();
			ConstantPropagation constPropOptimization = new ConstantPropagation();
			DeclarationOptimization declOptimization = new DeclarationOptimization();

			optimizations.Add(algOptimization);
			optimizations.Add(copyPropOptimization);
			optimizations.Add(constFoldingOptimization);
			optimizations.Add(constPropOptimization);
			optimizations.Add(declOptimization);

			return optimizations;
		}
		public List<Node> ApplyAllOptimizations(List<Node> nodes)
		{
			List<IOptimization> optimizations = CreateOptimizationList();

			for(int i = 0; i < optimizations.Count; i++)
			{
				int j = 0;
				optimizations[i].Optimize(nodes, out var applied);
				while(i > 0 && j != i)
				{
					optimizations[j].Optimize(nodes, out var applied2);
					j++;
				}
			}

			return nodes;
		}
	}
}
