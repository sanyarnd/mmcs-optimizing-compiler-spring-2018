using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA
{
    public class OptimizedGenericIterativeAlgorithm<T> : IAlgorithm<T>
    {
        public Func<T, T, bool> Comparer { get; set; }
        public Func<(T, T)> Fill { get; set; }
        public Func<T, string> DebugToString { get; set; }
        public Func<(T, T), (T, T), bool> Finish { get; set; }

        public IGraphNumerator Numerator { get; set; }

        public int OpsCount { get; set; }

        public InOutData<T> Analyze(
            ControlFlowGraph graph,
            ILatticeOperations<T> ops,
            ITransferFunction<T> f)
        {
            var data = new InOutData<T>
            {
                [graph.CFGNodes.ElementAt(0)] = Fill()
            };

            foreach (var node in graph.CFGNodes)
                data[node] = Fill();
            OpsCount = 0;
            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.CFGNodes.OrderBy(x => Numerator.GetIndex(x)))
                {
                    OpsCount++;
                    var inset = block.Parents.Aggregate(ops.Lower, (x, y)
                        => ops.Operator(x, data[y].Item2));
                    var outset = f.Transfer(block, inset, ops);
                    // Для отладки
                    var outStr = DebugToString?.Invoke(outset);
                    var inStr = DebugToString?.Invoke(inset);
                    // ------

                    if (!Finish((inset, outset), data[block]))
                    {
                        outChanged = true;
                    }
                    data[block] = (inset, outset);
                }
            }
            return data;
        }
    }
}
