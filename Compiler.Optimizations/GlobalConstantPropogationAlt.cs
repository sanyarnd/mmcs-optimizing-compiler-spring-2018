using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Optimizations
{
    public class GlobalConstantPropogationAlt
    {
        private readonly ThreeAddrCode.TACode _code;

        public GlobalConstantPropogationAlt(ThreeAddrCode.TACode code)
        {
            _code = code;
        }

        public ThreeAddrCode.TACode Optimize()
        {
            var cfg = new ControlFlowGraph(_code);
            var ops = new ThreeAddrCode.DFA.ConstantPropogationAlt.Operations(_code);
            var tf = new Compiler.ThreeAddrCode.DFA.ConstantPropogationAlt.TransferFunction(_code);
            var alg = new Compiler.ThreeAddrCode.DFA.ConstantPropogationAlt.IterativeAlgorithm(ops);
            var dfa = alg.Analyze(cfg, ops, tf);
            var res = new List<Node>();

            foreach (var bb in cfg.CFGNodes)
            {
                var bi = dfa[bb].Item1;
                var bres = bb.CodeList.ToList();
                for (int i = 0; i < bres.Count; i++)
                {
                    var line = bres[i];

                    if(line is Assign asn)
                    {
                        if(asn.Left is null && asn.Right is Var v)
                        {
                            var vi = bi[v.Id];
                            if (vi.IsConst)
                                asn.Right = new IntConst(vi.Value.Value);
                        }
                        else
                        {
                            if (asn.Right is Var v1)
                            {
                                var vi = bi[v1.Id];
                                if (vi.IsConst)
                                    asn.Right = new IntConst(vi.Value.Value);
                            }
                            if (asn.Left is Var v2)
                            {
                                var vi = bi[v2.Id];
                                if (vi.IsConst)
                                    asn.Left = new IntConst(vi.Value.Value);
                            }
                        }
                    }
                }
                res.AddRange(bres);
            }
            var code = new ThreeAddrCode.TACode();
            code.CodeList = res;
            AllOptimizations.LabelCode(code);
            return code;
        }
    }
}
