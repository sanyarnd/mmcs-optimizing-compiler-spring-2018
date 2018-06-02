﻿using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.DFA;
using Compiler.ThreeAddrCode.DFA.ReachingDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.IDE.Handlers
{
    internal class IterativeAlgorithmHandler
    {
        //public event EventHandler<string> InOutDataCollected = delegate { };
        public event EventHandler<string> PrintableInOutDataCollected = delegate { };

        public readonly Dictionary<IterativeAlgorithms, bool> IterativeAlgoList =
            new Dictionary<IterativeAlgorithms, bool>
            {
                {IterativeAlgorithms.ReachingDefs, false},
                {IterativeAlgorithms.ReachingExprs, false},
            };

        private Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>> AlgorithmResults(ControlFlowGraph cfg)
        {
            var algorithms = new Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>>();

            if (IterativeAlgoList[IterativeAlgorithms.ReachingDefs])
            {
                var op = new Operations(cfg.Code);
                var tf = new TransferFunction(cfg.Code);
                var reachingDefs = new GenericIterativeAlgorithm<HashSet<Guid>>()
                {
                    Finish = (a, b) =>
                    {
                        var (a1, a2) = a;
                        var (b1, b2) = b;

                        return !a2.Except(b2).Any();
                    },
                    Comparer = (x, y) => !x.Except(y).Any(),
                    Fill = () => (op.Lower, op.Lower)
                };
                var output = reachingDefs.Analyze(cfg,
                    new Operations(cfg.Code),
                    new TransferFunction(cfg.Code));
                algorithms.Add(IterativeAlgorithms.ReachingDefs, output);
            }

            if (IterativeAlgoList[IterativeAlgorithms.ReachingExprs])
            {
                var reachingExprs = new ThreeAddrCode.DFA.ReachingExpressions.IterativeAlgorithm();
                var output = reachingExprs.Analyze(cfg,
                    new ThreeAddrCode.DFA.ReachingExpressions.Operations(cfg.Code),
                    new ThreeAddrCode.DFA.ReachingExpressions.TransferFunction(cfg.Code));
                algorithms.Add(IterativeAlgorithms.ReachingExprs, output);
            }

            return algorithms;
        }

        public void CollectInOutData(ControlFlowGraph cfg)
        {
            var results = AlgorithmResults(cfg);
            string output = ResultsToString(cfg, results);
            PrintableInOutDataCollected(null, output);
        }

        private static string ResultsToString(ControlFlowGraph cfg,
            Dictionary<IterativeAlgorithms, InOutData<HashSet<Guid>>> dictionary)
        {
            var sb = new StringBuilder();
            foreach (var kvp in dictionary)
            {
                sb.AppendLine($"Алгоритм: {kvp.Key.GetString()}\n");
                foreach (var data in kvp.Value)
                {
                    sb.AppendLine($"{data.Key} = {{");

                    sb.AppendLine("    IN = {");
                    foreach (var val in data.Value.Item1)
                        sb.AppendLine($"        {cfg.Code.LabeledCode[val]}");
                    sb.AppendLine("    }");

                    sb.AppendLine();

                    sb.AppendLine("    OUT = {");
                    foreach (var val in data.Value.Item2)
                        sb.AppendLine($"        {cfg.Code.LabeledCode[val]}");
                    sb.AppendLine("    }");

                    sb.AppendLine("}\n");
                }
            }

            return sb.ToString();
        }
    }
}