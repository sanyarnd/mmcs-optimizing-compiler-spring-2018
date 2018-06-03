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
            var data = new InOutData<Dictionary<Guid, VarValue>>();
            foreach (var node in graph.CFGNodes)
                data[node] = (ops.Lower, ops.Lower);
            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.CFGNodes)
                {
                    var inset = block.Parents.Aggregate(ops.Lower, (x, y)
                        => ops.Operator(x, data[y].Item2));
                    var outset = f.Transfer(block, inset, ops);
                    if (outset.Count == data[block].Item2.Count && !outset.Except(data[block].Item2).Any())
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
