using Compiler.Parser.Visitors;
using System;
using Compiler.ThreeAddrCode;
using System.Collections.Generic;
using Compiler.Optimizations;
using System.Linq;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.IDE.Handlers
{
    internal class ThreeAddrCodeHandler
    {
        public readonly Dictionary<Optimizations, bool> OptimizationList = new Dictionary<Optimizations, bool>
        {
            {Optimizations.Alg, false},
            {Optimizations.Decl, false},
            {Optimizations.ConstProp, false},
            {Optimizations.CopyProp, false},
            {Optimizations.ConstFold, false},
            {Optimizations.Subexpr, false},
            {Optimizations.GlobalConstProp,false}
        };

        public bool RemoveDeadVariables = false;

        public event EventHandler<TACode> GenerationCompleted = delegate { };
        public event EventHandler<string> PrintableCodeGenerated = delegate { };
        public event EventHandler<Exception> GenerationErrored = delegate { };

        public void GenerateThreeAddrCode(object sender, Parser.AST.BlockNode root)
        {
            try
            {
                var visitor = new TACodeVisitor();
                root.Visit(visitor);

                TACode code = ApplyOptimizations(visitor.Code);
                GenerationCompleted(null, code);
                PostProcess(code);
            }
            catch (Exception ex)
            {
                GenerationErrored(null, ex);
            }
        }

        private List<IOptimization> BlockOptimizationList()
        {
            var optimizations = new List<IOptimization>();

            if (OptimizationList[Optimizations.Alg])
            {
                var algOptimization = new AlgebraicOptimization();
                optimizations.Add(algOptimization);
            }

            if (OptimizationList[Optimizations.Decl])
            {
                var declOptimization = new DeclarationOptimization();
                optimizations.Add(declOptimization);
            }

            if (OptimizationList[Optimizations.CopyProp])
            {
                var copyPropOptimization = new CopyPropagation();
                optimizations.Add(copyPropOptimization);
            }

            if (OptimizationList[Optimizations.ConstFold])
            {
                var constFoldingOptimization = new ConstantFolding();
                optimizations.Add(constFoldingOptimization);
            }

            if (OptimizationList[Optimizations.ConstProp])
            {
                var constPropOptimization = new ConstantPropagation();
                optimizations.Add(constPropOptimization);
            }

            if (OptimizationList[Optimizations.Subexpr])
            {
                var subexpOptimization = new SubexpressionOptimization();
                optimizations.Add(subexpOptimization);
            }

            return optimizations;
        }

        private TACode ApplyOptimizations(TACode code)
        {
            var optList = BlockOptimizationList();

            bool canApplyAny = true;

            TACode newCode = code;
            while (canApplyAny)
            {
                canApplyAny = false;
                var codeList = newCode.CreateBasicBlockList();
                newCode = new TACode();
                foreach (var b in codeList)
                {
                    List<Node> block = b.CodeList.ToList();

                    foreach (var opt in optList)
                    {
                        block = opt.Optimize(block, out var applied);
                        canApplyAny = canApplyAny || applied;
                    }

                    foreach (var node in block)
                    {
                        newCode.AddNode(node);
                    }
                }
                foreach (var line in newCode.CodeList)
                    newCode.LabeledCode[line.Label] = line;
            }

            if(OptimizationList[Optimizations.GlobalConstProp])
            {
                var opt = new GlobalConstantPropogationAlt(newCode);
                newCode = opt.Optimize();
                AllOptimizations.LabelCode(newCode);
            }

            if (RemoveDeadVariables)
            {
                var rmOpt = new RemoveDeadVariablesCFG(newCode);
                newCode = rmOpt.CodeNew;
            }


            return newCode;
        }

        private void PostProcess(TACode code)
        {
            string postProcessCode = OutputSanitizer.Sanitize(code.ToString(), OutputSanitizer.SanitizeLevel.TextBox);
            PrintableCodeGenerated(null, postProcessCode);
        }
    }
}