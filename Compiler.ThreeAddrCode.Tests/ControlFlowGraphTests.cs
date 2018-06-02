using System.Linq;
using Compiler.Parser;
using Compiler.Parser.Visitors;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.ThreeAddrCode.Tests
{
    [TestFixture]
    public class ControlFlowGraphTests
    {
        [Test]
        public void Test1()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new IntConst(3),
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
            ass3.IsLabeled = true;

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
            ass2.IsLabeled = true;

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

            var cfg = new ControlFlowGraph(taCode);
            var fst = cfg.CFGNodes.First();

            /**
             *   1) t1 = 3 - 5       -- 1-й блок-узел
             *   
             *   2) t2 = 10 + 2      -- 2-й блок-узел
             *   
             *   3) t3 = -1          -- 3-й блок-узел
             *   4) if 1 goto 3)
             *   
             *   5) t4 = t3 + 1999   -- 4-й блок-узел
             *   6) if 2 goto 2)
             *   
             *   7) t5 = 7 * 4       -- 5-й блок-узел
             *   8) t6 = 100 / 2
             * */

            //Тест 1-го узла
            Assert.AreEqual(fst.CodeList.ElementAt(0), taCode.CodeList.ElementAt(0));
            Assert.IsTrue(fst.CodeList.Count() == 1, "Number of TAC at first node is not 1");
            Assert.IsTrue(fst.Children.Count() == 1, "Number of children for first node is not 1");
            Assert.IsTrue(fst.Parents.Count() == 0, "Number of parents for first node is not 0");
            Assert.AreEqual(cfg.CFGNodes.ElementAt(1), fst.Children.ElementAt(0));

            //Тест 2-го узла
            Assert.IsTrue(cfg.CFGNodes.ElementAt(1).CodeList.Count() == 1, "Number of TAC at second node is not 1");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(1).Children.Count() == 1, "Number of children for second node is not 1");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(1).Parents.Count() == 2, "Number of parents for second node is not 1");
            Assert.AreEqual(cfg.CFGNodes.ElementAt(2), cfg.CFGNodes.ElementAt(1).Children.ElementAt(0)); //потомок для 2-го узла : 3-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(0), cfg.CFGNodes.ElementAt(1).Parents.ElementAt(1));  //предок для 2-го узла : 4-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(3), cfg.CFGNodes.ElementAt(1).Parents.ElementAt(0));  //предок для 2-го узла : 1-й узел

            //Тест 3-го узла
            Assert.IsTrue(cfg.CFGNodes.ElementAt(2).CodeList.Count() == 2, "Number of TAC at third node is not equals 2");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(2).Children.Count() == 2, "Number of children for third node is not equals 2");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(1).Parents.Count() == 2, "Number of parents for third node is not equals 2");
            Assert.AreEqual(cfg.CFGNodes.ElementAt(2), cfg.CFGNodes.ElementAt(2).Children.ElementAt(0)); //потомок для 3-го узла : 3-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(3), cfg.CFGNodes.ElementAt(2).Children.ElementAt(1)); //потомок для 3-го узла : 4-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(2), cfg.CFGNodes.ElementAt(2).Parents.ElementAt(0));  //предок для 3-го узла : 3-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(1), cfg.CFGNodes.ElementAt(2).Parents.ElementAt(1));  //предок для 3-го узла : 2-й узел

            //Тест 4-го узла
            Assert.IsTrue(cfg.CFGNodes.ElementAt(3).CodeList.Count() == 2, "Number of TAC at fourth node is not equals 2");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(3).Children.Count() == 2, "Number of children for fourth node is not equals 2");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(3).Parents.Count() == 1, "Number of parents for fourth node is not equals 1");
            Assert.AreEqual(cfg.CFGNodes.ElementAt(1), cfg.CFGNodes.ElementAt(3).Children.ElementAt(0)); //потомок для 4-го узла : 1-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(4), cfg.CFGNodes.ElementAt(3).Children.ElementAt(1)); //потомок для 4-го узла : 5-й узел
            Assert.AreEqual(cfg.CFGNodes.ElementAt(2), cfg.CFGNodes.ElementAt(3).Parents.ElementAt(0));  //предок для 4-го узла : 3-й узел

            //Тест 5-го узла
            Assert.IsTrue(cfg.CFGNodes.ElementAt(4).CodeList.Count() == 2, "Number of TAC at fifth node is not equals 2");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(4).Children.Count() == 0, "Number of children for fifth node is not equals 0");
            Assert.IsTrue(cfg.CFGNodes.ElementAt(4).Parents.Count() == 1, "Number of parents for fifth node is not equals 1");
            Assert.AreEqual(cfg.CFGNodes.ElementAt(3), cfg.CFGNodes.ElementAt(4).Parents.ElementAt(0));  //предок для 5-го узла : 4-й узел
        }
        [Test]
        public void Test2()
        {
            string unReducibleCFGProgram = 
            @"a = 7;
            goto h;
            while (3)
            {
                a = c;
                h: { c = a + b; }
                c = !(1 == 4);
            }
            a = 8;";
            var scanner = new Scanner();
            scanner.SetSource(unReducibleCFGProgram, 0);
            var parser = new Parser.Parser(scanner);
            var b = parser.Parse();
            var tacodeVisitor = new TACodeVisitor();
            var astRoot = parser.root;
            astRoot.Visit(tacodeVisitor);
            var tacodeInstance = tacodeVisitor.Code;
            var cfg = new ControlFlowGraph(tacodeInstance);
            Assert.IsFalse(cfg.IsReducible);


            string reducibleCFGProgram =
@"a = 7;
            goto h;
            while (3)
            {
                a = c;
                c = !(1 == 4);
            }
            h: { c = a + b; }
            a = 8;";

            scanner = new Scanner();
            scanner.SetSource(reducibleCFGProgram, 0);
            parser = new Parser.Parser(scanner);
            b = parser.Parse();
            tacodeVisitor = new TACodeVisitor();
            astRoot = parser.root;
            astRoot.Visit(tacodeVisitor);
            tacodeInstance = tacodeVisitor.Code;
            cfg = new ControlFlowGraph(tacodeInstance);
            Assert.IsTrue(cfg.IsReducible);
        }
 
    }
}