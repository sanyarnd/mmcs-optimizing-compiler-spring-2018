using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA.ReachingDefinitions
{
    public class IterativeAlgorithm : IAlgorithm<HashSet<Guid>>
    {
        public InOutData<HashSet<Guid>> Analyze(ControlFlowGraph graph, ILatticeOperations<HashSet<Guid>> ops, ITransferFunction<HashSet<Guid>> f)
        {
            var data = new InOutData<HashSet<Guid>>
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
                    if (outset.Except(data[block].Item2).Any())
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
