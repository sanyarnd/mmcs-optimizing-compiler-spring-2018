﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA
{
    public class GenericIterativeAlgorithm<T> : IAlgorithm<T>
    {
        public Func<T, T, bool> Comparer { get; set; }
        public Func<(T, T)> Fill { get; set; }
        public Func<T, string> DebugToString { get; set; }
        public Func<(T, T), (T, T), bool> Finish { get; set; }
        public int CountOfDoneIterations { get; private set; } = 0;
        public InOutData<T> Analyze(
            ControlFlowGraph graph,
            ILatticeOperations<T> ops,
            ITransferFunction<T> f)
        {
            var data = new InOutData<T>();
            data[graph.CFGNodes.ElementAt(0)] = Fill();

            foreach (var node in graph.CFGNodes)
                data[node] = Fill();

            var outChanged = true;
            CountOfDoneIterations = 0;
            while (outChanged)
            {
                CountOfDoneIterations++;
                outChanged = false;
                foreach (var block in graph.CFGNodes)
                {
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
