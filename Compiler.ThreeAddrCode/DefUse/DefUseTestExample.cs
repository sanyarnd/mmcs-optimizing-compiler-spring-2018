using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode
{
    public class DefUseTestExample
    {
        // По хорошему нужно сделать конструктор для Assign, но его не определяли
        // + Для типа операции нет значения, при котором ее не будет, например для строки a = 6
        // операция должна быть null, нужно ввести стандартное значение - я использовал просто not,
        // но нужно исправить
        /// <summary>
        /// Создает команду - Присваивания
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Operation"></param>
        /// <returns></returns>
        private Assign AssignC(Var Result, Expr Left, Expr Right = null, OpCode Operation = OpCode.Not, bool IsLabeled = true)
        {
            var C = new Assign();

            C.Left = Left;
            C.Right = Right;
            C.Result = Result;
            C.Operation = Operation;
            C.IsLabeled = IsLabeled;

            return C;
        }

        /// <summary>
        /// Создает команду - Печать переменной
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Sep"></param>
        /// <returns></returns>
        private Print PrintC(Var Data, string Sep = " ", bool IsLabeled = true)
        {
            var C = new Print();
            C.Data = Data;
            C.Sep = Sep;
            C.IsLabeled = IsLabeled;

            return C;
        }

        /// <summary>
        /// Создает команду - IfGoto (переход по условию)
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        private IfGoto IfGotoC(Expr Condition)
        {
            var C = new IfGoto();
            C.Condition = Condition;
            C.TargetLabel = new Guid();
            C.IsLabeled = true;

            return C;
        }

        /// <summary>
        /// Вызов всех тестов
        /// </summary>
        /// <returns></returns>
        public bool CorrectFunction()
        {
            // Здесь вызываются тесты
            return Test1() && Test2() && Test3() && Test4();
        }

        /// <summary>
        /// ---- Test 1
        /// </summary>
        /// <returns></returns>
        public bool Test1()
        {
            // -------------------------------------------------------------------
            // Создание базового блока
            // -------------------------------------------------------------------
            var a = new Var("a");
            var y = new Var("y");
            var x = new Var("x");

            BasicBlock B = new BasicBlock(new List<Node> {
                AssignC(a, new IntConst(5)),                     // 0: a = 5
                AssignC(x, a),                                   // 1: x = a
                AssignC(a, new IntConst(4)),                     // 2: a = 4
                AssignC(x, new IntConst(3)),                     // 3: x = 3
                AssignC(y, x, new IntConst(5), OpCode.Plus),     // 4: y = x + 7
                PrintC(y),                                       // 5: print(y)
                PrintC(x)                                        // 6: print(x) 
            });

            // -------------------------------------------------------------------
            // Тест для DefUse цепочек
            // -------------------------------------------------------------------

            DULists DL = new DULists(B);

            // Use цепочка
            var useList = new List<UNode> {
                new UNode(new DUVar(a, 1), new DUVar(a, 0)),
                new UNode(new DUVar(x, 4), new DUVar(x, 3)),
                new UNode(new DUVar(y, 5), new DUVar(y, 4)),
                new UNode(new DUVar(x, 6), new DUVar(x, 3))
            };

            // Def цепочка
            var defList = new List<DNode> {

                new DNode(new DUVar(a, 0),
                          new List<DUVar> {
                            new DUVar(a, 1)
                          }),

                new DNode(x, 1),

                new DNode(a, 2),

                new DNode(new DUVar(x, 3),
                          new List<DUVar> {
                            new DUVar(x, 4),
                            new DUVar(x, 6)
                          }),

                new DNode(new DUVar(y, 4),
                          new List<DUVar> {
                            new DUVar(y, 5)
                          }),
            };

            var IsUse = useList.Count == DL.UList.Count;
            var IsDef = defList.Count == DL.DList.Count;

            if (IsUse)
                for (var i = 0; i < DL.UList.Count; i++)
                    IsUse &= useList.Contains(DL.UList[i]);

            if (IsDef)
                for (var i = 0; i < DL.DList.Count; i++)
                    IsDef &= defList.Contains(DL.DList[i]);

            // -------------------------------------------------------------------
            // Тест для LiveAndDeadVariables
            // -------------------------------------------------------------------

            LiveAndDeadVariables LDV = new LiveAndDeadVariables(B);

            // Dead Vars
            var deadVars = new List<DUVar> {
                new DUVar(x, 1),
                new DUVar(a, 2)
            };

            // Live Vars
            var liveVars = new List<DUVar> {
                new DUVar(a, 0),
                new DUVar(a, 1),
                new DUVar(x, 3),
                new DUVar(x, 4),
                new DUVar(x, 6),
                new DUVar(y, 4),
                new DUVar(y, 5)
            };

            var IsDead = deadVars.Count == LDV.DeadVars.Count;
            var IsLive = liveVars.Count == LDV.LiveVars.Count;

            if (IsDead)
                for (var i = 0; i < LDV.DeadVars.Count; i++)
                    IsDead &= deadVars.Contains(LDV.DeadVars[i]);

            if (IsLive)
                for (var i = 0; i < LDV.LiveVars.Count; i++)
                    IsLive &= liveVars.Contains(LDV.LiveVars[i]);

            // -------------------------------------------------------------------
            // Тест для удаление мертвого кода
            // -------------------------------------------------------------------

            var B1 = LDV.BlockNew;

            BasicBlock B2 = new BasicBlock(new List<Node> {
                B.CodeList.ElementAt(3),                         // 0: x = 3
                B.CodeList.ElementAt(4),                         // 1: y = x + 5
                B.CodeList.ElementAt(5),                         // 2: print(y)
                B.CodeList.ElementAt(6)                          // 3: print(x) 
            });

            var IsEqBlocks = B1.CodeList.Count() == B2.CodeList.Count();
            foreach (var command in B2.CodeList)
                IsEqBlocks &= B1.CodeList.Contains(command);

            // Должна получиться истина
            return IsUse && IsDef && IsDead && IsLive && IsEqBlocks;
        }

        /// <summary>
        /// ---- Test 2
        /// </summary>
        /// <returns></returns>
        public bool Test2()
        {
            // -------------------------------------------------------------------
            // Создание базового блока
            // -------------------------------------------------------------------
            var a = new Var("a");
            var y = new Var("y");
            var x = new Var("x");

            BasicBlock B = new BasicBlock(new List<Node> {
                AssignC(a, new IntConst(5)),                     // 0: a = 5
                AssignC(x, a, new IntConst(4), OpCode.Plus),     // 1: x = a + 4
                AssignC(y, new IntConst(4), x, OpCode.Plus),     // 2: y = 4 + x
                AssignC(a, x, a, OpCode.Plus),                   // 3: a = x + a
                PrintC(y),                                       // 4: print(y)
                IfGotoC(a)                                       // 5: if (a) goto somewere 
            });

            // -------------------------------------------------------------------
            // Тест для DefUse цепочек
            // -------------------------------------------------------------------

            DULists DL = new DULists(B);

            // Use цепочка
            var useList = new List<UNode> {
                new UNode(new DUVar(a, 1), new DUVar(a, 0)),
                new UNode(new DUVar(x, 2), new DUVar(x, 1)),
                new UNode(new DUVar(x, 3), new DUVar(x, 1)),
                new UNode(new DUVar(a, 3), new DUVar(a, 0)),
                new UNode(new DUVar(y, 4), new DUVar(y, 2)),
                new UNode(new DUVar(a, 5), new DUVar(a, 3))
            };

            // Def цепочка
            var defList = new List<DNode> {

                new DNode(new DUVar(a, 0),
                          new List<DUVar> {
                            new DUVar(a, 1),
                            new DUVar(a, 3)
                          }),

                new DNode(new DUVar(x, 1),
                          new List<DUVar> {
                            new DUVar(x, 2),
                            new DUVar(x, 3)
                          }),

                new DNode(new DUVar(y, 2),
                          new List<DUVar> {
                            new DUVar(y, 4)
                          }),

                new DNode(new DUVar(a, 3),
                          new List<DUVar> {
                            new DUVar(a, 5)
                          }),
            };

            var IsUse = useList.Count == DL.UList.Count;
            var IsDef = defList.Count == DL.DList.Count;

            if (IsUse)
                for (var i = 0; i < DL.UList.Count; i++)
                    IsUse &= useList.Contains(DL.UList[i]);

            if (IsDef)
                for (var i = 0; i < DL.DList.Count; i++)
                    IsDef &= defList.Contains(DL.DList[i]);

            // -------------------------------------------------------------------
            // Тест для LiveAndDeadVariables
            // -------------------------------------------------------------------

            LiveAndDeadVariables LDV = new LiveAndDeadVariables(B);

            // Dead Vars
            var deadVars = new List<DUVar>();

            // Live Vars
            var liveVars = new List<DUVar> {
                new DUVar(a, 0),
                new DUVar(x, 1),
                new DUVar(a, 1),
                new DUVar(y, 2),
                new DUVar(x, 2),
                new DUVar(a, 3),
                new DUVar(x, 3),
                new DUVar(a, 3),
                new DUVar(y, 4),
                new DUVar(a, 5)
            };

            var IsDead = deadVars.Count == LDV.DeadVars.Count;
            var IsLive = liveVars.Count == LDV.LiveVars.Count;

            if (IsDead)
                for (var i = 0; i < LDV.DeadVars.Count; i++)
                    IsDead &= deadVars.Contains(LDV.DeadVars[i]);

            if (IsLive)
                for (var i = 0; i < LDV.LiveVars.Count; i++)
                    IsLive &= liveVars.Contains(LDV.LiveVars[i]);

            // -------------------------------------------------------------------
            // Тест для удаление мертвого кода
            // -------------------------------------------------------------------

            var B1 = LDV.BlockNew;

            var IsEqBlocks = B1.CodeList.Count() == B.CodeList.Count();
            foreach (var command in B.CodeList)
                IsEqBlocks &= B1.CodeList.Contains(command);

            // Должна получиться истина
            return IsUse && IsDef && IsDead && IsLive && IsEqBlocks;
        }

        /// <summary>
        /// ---- Test 3
        /// </summary>
        /// <returns></returns>
        public bool Test3()
        {
            // -------------------------------------------------------------------
            // Создание базового блока
            // -------------------------------------------------------------------
            var i = new Var("i");
            var j = new Var("j");
            var k = new Var("k");
            var l = new Var("l");

            BasicBlock B = new BasicBlock(new List<Node> {
                AssignC(i, k, new IntConst(1), OpCode.Plus),     // 0: i = k + 1
                AssignC(j, l, new IntConst(1), OpCode.Plus),     // 1: j = l + 1
                AssignC(k, i),                                   // 2: k = i
                AssignC(l, j)                                    // 3: l = j
            });

            // -------------------------------------------------------------------
            // Тест для DefUse множеств
            // -------------------------------------------------------------------

            DUSets DS = new DUSets(B);
            var defS = DS.DSet;
            var useS = DS.USet;

            B = new BasicBlock(new List<Node> {
                AssignC(i, i, new IntConst(1), OpCode.Plus),     // 0: i = i + 1
                AssignC(j, j, new IntConst(1), OpCode.Plus),     // 1: j = j + 1
            });

            // -------------------------------------------------------------------
            // Тест для DefUse множеств
            // -------------------------------------------------------------------

            DS = new DUSets(B);
            defS = DS.DSet;
            useS = DS.USet;

            // Должна получиться истина
            return true;
        }

        /// <summary>
        /// ---- Test 4
        /// </summary>
        /// <returns></returns>
        public bool Test4()
        {
            // -------------------------------------------------------------------
            // Создание базового блока
            // -------------------------------------------------------------------
            var a = new Var("a");
            var b = new Var("b");
            var c = new Var("c");

            BasicBlock B = new BasicBlock(new List<Node> {
                AssignC(a, new IntConst(2), IsLabeled: false),   // 0:       a = 2
                AssignC(b, new IntConst(3), IsLabeled: false),   // 1:       b = 3
                AssignC(c, a, b, OpCode.Plus),                   // 2: (1) : c = a + b
                AssignC(a, new IntConst(3)),                     // 3: (2) : a = 3
                AssignC(b, new IntConst(4), IsLabeled: false),   // 4:       b = 4
                AssignC(c, a),                                   // 5: (3) : c = a
            });

            // -------------------------------------------------------------------
            // Тест для итерационного алогритма Активных Переменных
            // -------------------------------------------------------------------

            var TA = new TACode();
            TA.CodeList = B.CodeList.ToList();
            var CFG = new ControlFlowGraph(TA);
            IterativeAlgorithmAV ItAV = new IterativeAlgorithmAV(CFG);

            // Должна получиться истина
            return true;
        }

        /// <summary>
        /// ---- Test 5
        /// </summary>
        /// <returns></returns>
        public bool Test5()
        {
            // -------------------------------------------------------------------
            // Создание базового блока
            // -------------------------------------------------------------------
            var a = new Var("a");
            var b = new Var("b");
            var c = new Var("c");

            BasicBlock B = new BasicBlock(new List<Node> {
                AssignC(a, new IntConst(2), IsLabeled: false),   // 0:       a = 2
                AssignC(b, new IntConst(3), IsLabeled: false),   // 1:       b = 3
                AssignC(c, a, b, OpCode.Plus),                   // 2: (1) : c = a + b
                AssignC(a, new IntConst(3)),                     // 3: (2) : a = 3
                AssignC(b, new IntConst(4), IsLabeled: false),   // 4:       b = 4
                AssignC(c, a),                                   // 5: (3) : c = a
                PrintC(c, IsLabeled: false)                                        // 6:       print(c)
            });

            // -------------------------------------------------------------------
            // Тест для удаления мертвого кода в CFG
            // -------------------------------------------------------------------

            var TA = new TACode();
            TA.CodeList = B.CodeList.ToList();
            var RCFG = new RemoveDeadVariablesCFG(TA);

            // Результат
            var newCode = RCFG.CodeNew;

            // Должна получиться истина
            return true;
        }
    }
}
