using Compiler.ThreeAddrCode;
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
    public class DeclarationTest
    {
        public void DeclarationOptimizationTest()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new IntConst(30),
                Operation = OpCode.Minus,
                Right = new IntConst(5),
                Result = new Var()
            };
            ass1.IsLabeled = true;
            var ass2 = new Assign
            {
                Left = new IntConst(3),
                Operation = OpCode.Mul,
                Right = new IntConst(4),
                Result = new Var()
            };

            var ass3 = new Assign
            {
                Operation = OpCode.Plus,
                Left = ass2.Result,
                Right = new IntConst(5),
                Result = new Var()
            };
            var ass4 = new Assign
            {
                Operation = OpCode.Plus,
                Left = ass1.Result,
                Right = new IntConst(1),
                Result = new Var()
            };
            var ass5 = new Assign
            {
                Operation = OpCode.Mul,
                Left = new IntConst(4),
                Right = new IntConst(7),
                Result = new Var()
            };

            var ass6 = new Assign
            {
                Operation = OpCode.Mul,
                Left = new IntConst(8),
                Right = new IntConst(9),
                Result = new Var()
            };

            var ass7 = new Assign
            {
                Operation = OpCode.Mul,
                Left = ass3.Result,
                Right = new IntConst(3),
                Result = new Var()
            };
            var ifgt1 = new IfGoto
            {
                Condition = ass1.Result,
                TargetLabel = ass1.Label
            };

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ass4);
            taCode.AddNode(ass5);
            taCode.AddNode(ass6);
            taCode.AddNode(ass7);
            taCode.AddNode(ifgt1);

            Console.WriteLine($"TA Code\n: {taCode}");
            var algOpt = new DeclarationOptimization();

            Console.WriteLine($"TA Code\n");
            foreach (var node in algOpt.Optimize(taCode.CodeList.ToList(), out var applied))
                Console.WriteLine($"{node}");
            Console.ReadKey();

            //var bBlocks = taCode.CreateBasicBlockList();
            //foreach (var bbl in bBlocks)
            //{
            //    Console.WriteLine($"Block[{bbl.BlockId}]:");
            //    var bblCodeStr = bbl.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
            //    Console.WriteLine($"{bblCodeStr}\n");
            //}

            //Console.WriteLine($"Algebraic Optimization was{(applied ? "" : "n't")} applied");

            //Console.WriteLine("========================CFG test========================");
            //var cfg = new ControlFlowGraph(taCode);
            //foreach (var cfgn in cfg.CFGNodes)
            //{
            //    Console.WriteLine($"Block[{cfgn.Block.BlockId}]");
            //    var bblCodeStr = cfgn.Block.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
            //    Console.WriteLine($"{bblCodeStr}\n");

            //    Console.WriteLine("Children:\n");
            //    foreach (var ch in cfgn.Children)
            //    {
            //        Console.WriteLine($"Block[{ch.Block.BlockId}]");
            //        var bblCodeCh = ch.Block.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
            //        Console.WriteLine($"{bblCodeCh}\n");
            //    }

            //    Console.WriteLine("Parents:\n");
            //    foreach (var ch in cfgn.Parents)
            //    {
            //        Console.WriteLine($"Block[{ch.Block.BlockId}]");
            //        var bblCodeCh = ch.Block.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
            //        Console.WriteLine($"{bblCodeCh}\n");
            //    }
            //    Console.WriteLine("-----------------------------------------");
            //}
        }
    }
}