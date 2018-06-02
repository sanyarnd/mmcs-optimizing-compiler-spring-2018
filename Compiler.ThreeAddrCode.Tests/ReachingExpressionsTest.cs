using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.DFA.ReachingExpressions;


namespace Compiler.ThreeAddrCode.Tests
{
    [TestFixture]
    class ReachingExpressionsTest
    {
        [Test]
        public void ReachingExpressionsTest1()
        {
            var taCode = new TACode();

            //TAC выглядит следующим образом
            /**
             * 0)   t0 = 3            |
             * 1)   t1 = 5 + t0       | B1
             * 2)   t2 = t1 + t0      |
             * 
             * 
             * 3)   t3 = 4 + t2       |
             * 4)   t1 = 10           | B2
             * 5)   if (1) goto 3)    |
             * 
             * 
             * 6)   t4 = t1 + 5       |
             * 7)   t5 = t3 + t0      |
             * 8)   t0 = 100          | B3
             * 9)   if (2) goto 6)    |
             * 
             * 
             * 10)  t6 = t5 + 10      |
             * 11)  t7 = t6 + 10      |
             * 12)  t8 = t6 + t7      | B4
             * 13)  t6 = 3            |
             * 14)  t5 = 100          |
             * 
             **/

            var ass1 = new Assign
            {
                Operation = OpCode.Plus,
                Right = new IntConst(3),
                Result = new Var()
            };

            var ass2 = new Assign
            {
                Left = new IntConst(5),
                Operation = OpCode.Plus,
                Right = ass1.Result,
                Result = new Var()
            };

            var ass3 = new Assign
            {
                Left = ass2.Result,
                Operation = OpCode.Plus,
                Right = ass1.Result,
                Result = new Var()
            };

            var ass4 = new Assign
            {
                Left = new IntConst(4),
                Operation = OpCode.Plus,
                Right = ass3.Result,
                Result = new Var()
            };

            var ass5 = new Assign
            {
                Operation = OpCode.Plus,
                Right = new IntConst(10),
                Result = ass2.Result
            };

            var ifgt1 = new IfGoto
            {
                Condition = new IntConst(1),
                TargetLabel = ass4.Label
            };
            ass4.IsLabeled = true;

            var ass6 = new Assign
            {
                Left = ass2.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(5),
                Result = new Var()
            };

            var ass7 = new Assign
            {
                Left = ass4.Result,
                Operation = OpCode.Plus,
                Right = ass1.Result,
                Result = new Var()
            };

            var ass8 = new Assign
            {
                Operation = OpCode.Plus,
                Right = new IntConst(100),
                Result = ass1.Result
            };

            var ifgt2 = new IfGoto
            {
                Condition = new IntConst(2),
                TargetLabel = ass6.Label
            };
            ass6.IsLabeled = true;

            var ass9 = new Assign
            {
                Left = ass7.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(10),
                Result = new Var()
            };

            var ass10 = new Assign
            {
                Left = ass9.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(10),
                Result = new Var()
            };

            var ass11 = new Assign
            {
                Left = ass9.Result,
                Operation = OpCode.Plus,
                Right = ass10.Result,
                Result = new Var()
            };

            var ass12 = new Assign
            {
                Operation = OpCode.Plus,
                Right = new IntConst(3),
                Result = ass9.Result
            };

            var ass13 = new Assign
            {
                Operation = OpCode.Plus,
                Right = new IntConst(100),
                Result = ass7.Result
            };

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ass4);
            taCode.AddNode(ass5);
            taCode.AddNode(ifgt1);
            taCode.AddNode(ass6);
            taCode.AddNode(ass7);
            taCode.AddNode(ass8);
            taCode.AddNode(ifgt2);
            taCode.AddNode(ass9);
            taCode.AddNode(ass10);
            taCode.AddNode(ass11);
            taCode.AddNode(ass12);
            taCode.AddNode(ass13);

            /**         CFG имеет следующий вид
             *                                   
             *                  B1
             *                   |
             *                   |   ____
             *                   v  /    \
             *                  B2-v______|
             *                   |
             *                   |   ____
             *                   v  /    \
             *                  B3-v______|
             *                   |   
             *                   |
             *                   V
             *                  B4
             * */

            var cfg = new ControlFlowGraph(taCode);

            /**************************Reaching definition test*************************/
            var op = new Operations(taCode);
            var algo = new IterativeAlgorithm();
            var transFunc = new TransferFunction(taCode);
            var inout = algo.Analyze(cfg, op, transFunc);

            //Тестирование множест e_gen и e_kill для каждого базового блока
            var (e_gen, e_kill) = transFunc.GetEGenEKill(cfg.CFGNodes.ElementAt(0));
            Assert.True(e_gen.SetEquals(new HashSet<Guid> { ass2.Label, ass3.Label }));
            Assert.True(e_kill.SetEquals(new HashSet<Guid> { ass4.Label, ass6.Label, ass7.Label }));

            (e_gen, e_kill) = transFunc.GetEGenEKill(cfg.CFGNodes.ElementAt(1));
            Assert.True(e_gen.SetEquals(new HashSet<Guid> { ass4.Label }));
            Assert.True(e_kill.SetEquals(new HashSet<Guid> { ass3.Label, ass6.Label, ass7.Label }));

            (e_gen, e_kill) = transFunc.GetEGenEKill(cfg.CFGNodes.ElementAt(2));
            Assert.True(e_gen.SetEquals(new HashSet<Guid> { ass6.Label }));
            Assert.True(e_kill.SetEquals(new HashSet<Guid> { ass2.Label, ass3.Label, ass7.Label, ass9.Label }));

            (e_gen, e_kill) = transFunc.GetEGenEKill(cfg.CFGNodes.ElementAt(3));
            Assert.True(e_gen.SetEquals(new HashSet<Guid> { }));
            Assert.True(e_kill.SetEquals(new HashSet<Guid> { ass9.Label, ass10.Label, ass11.Label }));

            //Текстирование IN/OUT множеств для каждого ББл
            var trueInOut = new DFA.InOutData<HashSet<System.Guid>>();
            trueInOut.Add(cfg.CFGNodes.ElementAt(0),
                (new HashSet<Guid>(),
                 new HashSet<Guid> { ass2.Label, ass3.Label })
            );
            trueInOut.Add(cfg.CFGNodes.ElementAt(1),
                (new HashSet<Guid> { ass2.Label, ass3.Label },
                new HashSet<Guid> { ass2.Label, ass4.Label })
            );
            trueInOut.Add(cfg.CFGNodes.ElementAt(2),
                (new HashSet<Guid> { ass2.Label, ass4.Label },
                new HashSet<Guid> { ass4.Label, ass6.Label })
            );
            trueInOut.Add(cfg.CFGNodes.ElementAt(3),
                (new HashSet<Guid> { ass4.Label, ass6.Label },
                new HashSet<Guid> { ass4.Label, ass6.Label })
            );

            foreach (var x in cfg.CFGNodes)
            {
                HashSet<Guid> toutItem1 = trueInOut[x].Item1;
                HashSet<Guid> outItem1 = inout[x].Item1;
                HashSet<Guid> toutItem2 = trueInOut[x].Item2;
                HashSet<Guid> outItem2 = inout[x].Item2;

                var inEq = toutItem1.SetEquals(outItem1);
                var outEq = toutItem2.SetEquals(outItem2);
                Assert.True(inEq && outEq);
            }
        }
    }
}
