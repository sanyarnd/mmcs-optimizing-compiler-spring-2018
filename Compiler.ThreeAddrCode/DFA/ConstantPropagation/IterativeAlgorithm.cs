using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA.ConstantPropagation
{
    public class IterativeAlgorithm : IAlgorithm<Dictionary<Guid, VarValue>>
    {
        public InOutData<Dictionary<Guid, VarValue>> Analyze(ControlFlowGraph graph, ILatticeOperations<Dictionary<Guid, VarValue>> ops, ITransferFunction<Dictionary<Guid, VarValue>> f)
        {
            var data = new InOutData<Dictionary<Guid, VarValue>>
            {
                [graph.CFGNodes.ElementAt(0)] = (
        ops.Lower,
        f.Transfer(graph.CFGNodes.ElementAt(0), ops.Lower, ops)
    )
            };
            foreach (var node in graph.CFGNodes.Skip(1))
                data[node] = (ops.Lower, ops.Lower);
            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.CFGNodes.Skip(1))
                {
                    var inset = block.Parents.Aggregate(ops.Lower, (x, y)
                        => ops.Operator(x, data[y].Item2));
                    var outset = f.Transfer(block, inset, ops);
                    if (outset == data[block].Item2)
                    {
                        outChanged = true;
                        data[block] = (inset, outset);
                    }
                }
            }
            return data;
        }
    }
}
