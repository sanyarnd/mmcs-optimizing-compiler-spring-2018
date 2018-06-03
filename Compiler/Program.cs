using Compiler.Parser;
using Compiler.Parser.Visitors;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.Optimizations;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Compiler.Parser.AST;
using Compiler.ILcodeGenerator;
using System.Collections.Generic;

namespace Compiler
{
    internal class Program
    {
        public static void Main()
        {
            //TaCodeTest();
            //ASTTest();

            //Test Moving declarations
            //var Test = new DeclarationTest();
            //Test.DeclarationOptimizationTest();

            //Test subexpression tree
            //var sTest = new SubexprTest();
            //sTest.SubexpressionOptimizationTest();

            //APC
            //CFGReducibility_DominatorTree_PrettyPrinter_Demonstration();

            string fileName = @"..\..\sample.txt";

            astRoot = AST(fileName);
            if (astRoot == null)
                return;

            var tacodeVisitor = new TACodeVisitor();
            astRoot.Visit(tacodeVisitor);
            tacodeInstance = tacodeVisitor.Code;

            var allOpt = new AllOptimizations();
            tacodeInstance = allOpt.ApplyAllOptimizations(tacodeInstance);

            TAcode2ILcodeTranslator trans = new TAcode2ILcodeTranslator();

            Console.WriteLine(tacodeInstance.ToString());

            trans.Translate(tacodeInstance);
            var temp = trans.PrintCommands();
            trans.RunProgram();
        }

        private static BlockNode astRoot;
        private static TACode tacodeInstance;

        private static void CFGReducibility_DominatorTree_PrettyPrinter_Demonstration()
        {
            string fpath = @"..\..\..\CodeSamples\reducibilityBadSample.txt";
            astRoot = AST(fpath);
            if (astRoot == null)
                return;
            var tacodeVisitor = new TACodeVisitor();
            astRoot.Visit(tacodeVisitor);
            var prettyPrinter = new PrettyPrintVisitor();
            astRoot.Visit(prettyPrinter);
            var cfg = new ControlFlowGraph(tacodeVisitor.Code);
            var domTree = new DominatorTree(cfg);
            Console.WriteLine("###### CFG Reducibility(#59 by APC TEAM) based on DominatorTree(#56 by ДВП)");
            Console.WriteLine("###### and PrettyPrinter(#5 by APC TEAM) DEMONSTARTION:");
            Console.WriteLine("######       Sample 1:");
            Console.WriteLine(prettyPrinter.Text);
            Console.WriteLine("###### Dominator Tree Matrix:");
            Console.WriteLine(domTree.ToString());
            Console.WriteLine($"###### CFG is reducible: {cfg.IsReducible}");

            fpath = @"..\..\..\CodeSamples\reducibilityGoodSample.txt";
            astRoot = null;
            astRoot = AST(fpath);
            if (astRoot == null)
                return;
            tacodeVisitor = new TACodeVisitor();
            astRoot.Visit(tacodeVisitor);
            prettyPrinter = new PrettyPrintVisitor();
            astRoot.Visit(prettyPrinter);
            cfg = new ControlFlowGraph(tacodeVisitor.Code);
            domTree = new DominatorTree(cfg);
            Console.WriteLine("###### CFG Reducibility(#59 by APC TEAM) based on DominatorTree(#56 by ДВП)");
            Console.WriteLine("###### and PrettyPrinter(#5 by APC TEAM) DEMONSTARTION:");
            Console.WriteLine("######       Sample 2:");
            Console.WriteLine("###### Program text from PrettyPrinter:\n");
            Console.WriteLine(prettyPrinter.Text);
            Console.WriteLine("###### Dominator Tree Matrix:");
            Console.WriteLine(domTree.ToString());
            Console.WriteLine($"###### CFG is reducible: {cfg.IsReducible}");
        }

        private static BlockNode AST(string fileName)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                string text = File.ReadAllText(fileName);

                var scanner = new Scanner();
                scanner.SetSource(text, 0);

                var parser = new Parser.Parser(scanner);

                var b = parser.Parse();
                if (b)
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    return parser.root;
                }
                else
                {
                    Console.WriteLine("Ошибка");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл {fileName} не найден");
            }
            catch (LexException e)
            {
                Console.WriteLine($"Лексическая ошибка. {e.Message}");
            }
            catch (SyntaxException e)
            {
                Console.WriteLine($"Синтаксическая ошибка. {e.Message}");
            }
            return null;
        }

        private static void TaCodeTest()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new IntConst(3),
                //Left = new IntConst(5),
                Operation = OpCode.Minus,
                Right = new IntConst(5),
                Result = new Var()
            };

            var ass2 = new Assign
            {
                Left = new IntConst(10),
                Operation = OpCode.Plus,
                Right = new IntConst(2),
                Result = new Var()
            };

            var ass3 = new Assign
            {
                Operation = OpCode.Minus,
                Right = new IntConst(1),
                Result = new Var()
            };

            var ifgt1 = new IfGoto
            {
                Condition = new IntConst(1),
                TargetLabel = ass3.Label
            };
            ass3.IsLabeled = true; //На этот оперетор мы переходим по условию

            var ass4 = new Assign
            {
                Left = ass3.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(1999),
                Result = new Var()
            };

            var ifgt2 = new IfGoto
            {
                Condition = new IntConst(2),
                TargetLabel = ass2.Label
            };
            ass2.IsLabeled = true; //На этот оперетор мы переходим по условию

            var ass5 = new Assign
            {
                Left = new IntConst(7),
                Operation = OpCode.Mul,
                Right = new IntConst(4),
                Result = new Var()
            };

            var ass6 = new Assign
            {
                Left = new IntConst(100),
                Operation = OpCode.Div,
                Right = new IntConst(25),
                Result = new Var()
            };

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ifgt1);
            taCode.AddNode(ass4);
            taCode.AddNode(ifgt2);
            taCode.AddNode(ass5);
            taCode.AddNode(ass6);

            var algOpt = new AlgebraicOptimization();
            algOpt.Optimize(taCode.CodeList.ToList(), out var applied);

            Console.WriteLine($"TA Code\n: {taCode}");

            var bBlocks = taCode.CreateBasicBlockList();
            foreach (var bbl in bBlocks)
            {
                Console.WriteLine($"Block[{bbl.BlockId}]:");
                var bblCodeStr = bbl.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
                Console.WriteLine($"{bblCodeStr}\n");
            }

            Console.WriteLine($"Algebraic Optimization was{(applied ? "" : "n't")} applied");

            Console.WriteLine("========================CFG test========================");
            var cfg = new ControlFlowGraph(taCode);
            foreach (var cfgn in cfg.CFGNodes)
            {
                Console.WriteLine($"Block[{cfgn.BlockId}]");
                var bblCodeStr = cfgn.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
                Console.WriteLine($"{bblCodeStr}\n");

                Console.WriteLine("Children:\n");
                foreach (var ch in cfgn.Children)
                {
                    Console.WriteLine($"Block[{ch.BlockId}]");
                    var bblCodeCh = ch.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
                    Console.WriteLine($"{bblCodeCh}\n");
                }

                Console.WriteLine("Parents:\n");
                foreach (var ch in cfgn.Parents)
                {
                    Console.WriteLine($"Block[{ch.BlockId}]");
                    var bblCodeCh = ch.CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
                    Console.WriteLine($"{bblCodeCh}\n");
                }
                Console.WriteLine("-----------------------------------------");
            }
			//Copy Propagation Test
			var taCodeCopyProp = new TACode();
			var assgn1 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new Var(),
				Result = new Var()
			};
			var assgn2 = new Assign()
			{
				Left = assgn1.Right,
				Operation = OpCode.Minus,
				Right = assgn1.Result,
				Result = new Var()
			};
			var assgn3 = new Assign()
			{
				Left = assgn2.Result,
				Operation = OpCode.Plus,
				Right = new IntConst(1),
				Result = new Var()
			};
			var assgn4 = new Assign()
			{
				Left = assgn3.Result,
				Operation = OpCode.Mul,
				Right = assgn1.Result,
				Result = new Var()
			};
			var assgn5 = new Assign()
			{
				Left = new IntConst(30),
				Operation = OpCode.Minus,
				Right = new IntConst(20),
				Result = assgn1.Result
			};
			var assgn6 = new Assign()
			{
				Left = assgn2.Result,
				Operation = OpCode.Plus,
				Right = assgn5.Result,
				Result = new Var()
			};

			taCodeCopyProp.AddNode(assgn1);
			taCodeCopyProp.AddNode(assgn2);
			taCodeCopyProp.AddNode(assgn3);
			taCodeCopyProp.AddNode(assgn4);
			taCodeCopyProp.AddNode(assgn5);
			taCodeCopyProp.AddNode(assgn6);

			/*
			  a = b
			  c = b - a     -----> c = b - b
			  d = c + 1
			  e = d * a     -----> e = d * b
			  a = 30 - 20
			  k = c + a     -----> k = c + a
			*/

			Console.WriteLine($"Testing Copy Propagation Optimisation.\n Three Adress Code:\n {taCodeCopyProp}");
			var optCopyProp = new CopyPropagation();
			optCopyProp.Optimize(taCodeCopyProp.CodeList.ToList(), out var applCopProp);
			Console.WriteLine($"Optimisation Copy Propagation was{(applCopProp ? "" : "n't")} applied");
			Console.WriteLine($"Three Adress Code Code\n: {taCodeCopyProp}");
			Console.WriteLine("-----------------------------------------");

			//Constant Folding Test
			var taCodeConstantFolding = new TACode();
			var assign1 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new Var(),
				Result = new Var()
			};
			var assign2 = new Assign()
			{
				Left = new IntConst(20),
				Operation = OpCode.Mul,
				Right = new IntConst(3),
				Result = new Var()
			};
			var assign3 = new Assign()
			{
				Left = new IntConst(10),
				Operation = OpCode.Plus,
				Right = new IntConst(1),
				Result = new Var()
			};
			var assign4 = new Assign()
			{
				Left = new IntConst(100),
				Operation = OpCode.Div,
				Right = new IntConst(50),
				Result = new Var()
			};
			var assign5 = new Assign()
			{
				Left = new IntConst(30),
				Operation = OpCode.Minus,
				Right = new IntConst(20),
				Result = assign1.Result
			};
			var assign6 = new Assign()
			{
				Left = assign2.Result,
				Operation = OpCode.Plus,
				Right = assign5.Result,
				Result = new Var()
			};

			taCodeConstantFolding.AddNode(assign1);
			taCodeConstantFolding.AddNode(assign2);
			taCodeConstantFolding.AddNode(assign3);
			taCodeConstantFolding.AddNode(assign4);
			taCodeConstantFolding.AddNode(assign5);
			taCodeConstantFolding.AddNode(assign6);

			/*
			  a = b
			  c = 20 * 3    -----> c = 60
			  d = 10 + 1    -----> d = 11
			  e = 100 / 50  -----> e = 2
			  a = 30 - 20   -----> a = 10
			  k = c + a
			*/

			Console.WriteLine($"Testing Constant Folding Optimisation.\n Three Adress Code:\n {taCodeConstantFolding}");
			var optConstFold = new ConstantFolding();
			optConstFold.Optimize(taCodeConstantFolding.CodeList.ToList(), out var applConstFold);
			Console.WriteLine($"Optimisation Constant Folding was{(applConstFold ? "" : "n't")} applied");
			Console.WriteLine($"Three Adress Code Code\n: {taCodeConstantFolding}");
			Console.WriteLine("-----------------------------------------");

			//All Optimizations Together Test
			var taCodeAllOptimizations = new TACode();
			var a1 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new Var(),
				Result = new Var()
			};
			var a2 = new Assign()
			{
				Left = a1.Right,
				Operation = OpCode.Minus,
				Right = a1.Result,
				Result = new Var()
			};
			var a3 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new IntConst(20),
				Result = new Var()
			};
			var a4 = new Assign()
			{
				Left = new IntConst(20),
				Operation = OpCode.Mul,
				Right = new IntConst(3),
				Result = new Var()
			};
			var a5 = new Assign()
			{
				Left = new IntConst(10),
				Operation = OpCode.Plus,
				Right = a3.Result,
				Result = new Var()
			};
			taCodeAllOptimizations.AddNode(a1);
			taCodeAllOptimizations.AddNode(a2);
			taCodeAllOptimizations.AddNode(a3);
			taCodeAllOptimizations.AddNode(a4);
			taCodeAllOptimizations.AddNode(a5);

			/*
			  a = b
			  c = b - a   -----> c = 0
			  n = 20
			  c = 20 * 3  -----> c = 60
			  d = 10 + n  -----> d = 30
			*/

			Console.WriteLine($"Testing All Optimizations Together.\n Three Adress Code:\n {taCodeAllOptimizations}");
			var allOptimizations = new AllOptimizations();
			allOptimizations.ApplyAllOptimizations(taCodeAllOptimizations);
			Console.WriteLine($"Three Adress Code Code\n: {taCodeAllOptimizations}");
			Console.WriteLine("-----------------------------------------");
		}
	}
}