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
        public Assign AssignC(Var Result, Expr Left, Expr Right = null, OpCode Operation = OpCode.Not, PrettyGuid Label = null)
        {
            var C = new Assign();

            C.Left = Left;
            C.Right = Right;
            C.Result = Result;
            C.Operation = Operation;
            C.Label = Label;
            C.IsLabeled = C.Label == null ? false : true;

            return C;
        }

        // Печать переменной
        public Print PrintC(Var Data, string Sep = " ", PrettyGuid Label = null)
        {
            var C = new Print();
            C.Data = Data;
            C.Sep = Sep;
            C.Label = Label;
            C.IsLabeled = C.Label == null ? false : true;

            return C;
        }

        public void CorrectFunction()
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

            var IsUse = true;
            var IsDef = true;

            // Проверка
            for (var i = 0; i < DL.UList.Count; i++)
                IsUse &= DL.UList[i] == useList[i];

            for (var i = 0; i < DL.DList.Count; i++)
                IsDef &= DL.DList[i] == defList[i];
        }
    }
}
