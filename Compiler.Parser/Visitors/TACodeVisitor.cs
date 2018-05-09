using Compiler.Parser.AST;
using Compiler.ThreeAddrCode;
using System;
using System.Collections.Generic;
using TACExpr = Compiler.ThreeAddrCode.Expressions;
using TACNodes = Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Parser.Visitors
{
    /// <summary>
    ///     Визитор для обхода AST и генерации трехадресного кода
    /// </summary>
    public class TACodeVisitor : AutoVisitor
    {
        /// <summary>
        ///     Программа в виде списка команд трехадреного кода
        /// </summary>
        public TACode Code => code;

        /// <summary>
        /// Экземпляр программы в треадресном коде
        /// </summary>
        private readonly TACode code = new TACode();

        /// <summary>
        ///     Список команд безусловного перехода, ведущих в непреобразованную часть кода и ожидающих заполнения поля
        ///     TargetLabel, в формате ИмяМетки - Список команд перехода к этой метке
        /// </summary>
        private readonly Dictionary<string, List<TACNodes.Goto>> forwardGotos =
            new Dictionary<string, List<TACNodes.Goto>>();

        /// <summary>
        ///     Список помеченных команд(строк) трехадресного кода в формате ИмяМетки - Узел
        /// </summary>
        private readonly Dictionary<string, TACNodes.Node> labeledTANodes = new Dictionary<string, TACNodes.Node>();

        /// <summary>
        ///     Список переменных исходного кода в формате ИмяПеременной - Адрес в трехадресном коде
        /// </summary>
        private readonly Dictionary<string, TACExpr.Var> m_varsInCode = new Dictionary<string, TACExpr.Var>();

        public override void VisitLabeledNode(LabeledNode l)
        {
            string labelName = l.Label.Name;
            // Создаем пустой оператор и указываем, что на него есть переход по метке
            var labeledNop = GetEmptyLabeledNode(labelName);

            // Добавляем метку и помеченный оператор в список помеченных операторов (это всегда нужно делать,
            // т.к. дальше по тексту могут оказаться goto на данную метку)
            labeledTANodes.Add(labelName, labeledNop);

            // Проверяем, не было ли по этой метке переходов, преобразованных ранее
            if (forwardGotos.ContainsKey(labelName))
            {
                // Если были, заполняем их поля меток и удаляем из списка ожидания
                foreach (var ta_goto in forwardGotos[labelName])
                    ta_goto.TargetLabel = labeledNop.Label;
                forwardGotos.Remove(labelName);
            }

            // Продолжаем отдельно разбор помеченного оператора как обычного
            l.Stat.Visit(this);
        }

        public override void VisitGoToNode(GoToNode g)
        {
            string labelName = g.Label.Name;
            // При посещении узла GoTо создаем соответсвующую команду трехадресного кода
            var gt = new TACNodes.Goto();
            code.AddNode(gt);

            // Если метка ведет в уже преобразованную часть программы
            if (labeledTANodes.ContainsKey(labelName))
            {
                // Получаем помеченную строку треадресного кода и задаем ее как цель перехода
                var target = labeledTANodes[labelName];
                gt.TargetLabel = target.Label;
            }
            else
            {
                // Иначе помещаем строку в лист ожидания пока помеченная часть программы не будет преобразована
                if (!forwardGotos.ContainsKey(labelName))
                    forwardGotos.Add(labelName, new List<TACNodes.Goto>());
                forwardGotos[labelName].Add(gt);
            }
        }

        public override void VisitAssignNode(AssignNode a)
        {
            var v = GetVarByName(a.Id.Name);
            TACodeNameManager.Instance.Name(v.Id, a.Id.Name);

            var assign = new TACNodes.Assign
            {
                Left = null,
                Right = RecAssign(a.Expr),
                Result = GetVarByName(a.Id.Name),
                Operation = OpCode.Copy
            };

            code.AddNode(assign);
        }

        public override void VisitCycleNode(CycleNode c)
        {
            // Метка в начале цикла
            var cycleLabel = GetEmptyLabeledNode();

            // Результат вычисления логического выражения
            var cond = RecAssign(c.Condition);

            // При истинности условия, переходим к телу цикла
            var ifGotoBody = new TACNodes.IfGoto { Condition = cond };
            code.AddNode(ifGotoBody);

            // Иначе переходим за тело цикла
            var gotoEnd = new TACNodes.Goto();
            code.AddNode(gotoEnd);

            // Добавление новой метки непосредственно перед телом цикла
            var bodyLabel = GetEmptyLabeledNode();
            ifGotoBody.TargetLabel = bodyLabel.Label;

            // Обход выражений тела цикла
            c.Body.Visit(this);

            // В конце цикла снова переходим к началу
            var cycleGoto = new TACNodes.Goto { TargetLabel = cycleLabel.Label };
            code.AddNode(cycleGoto);

            // Метка за телом цикла, сюда происходит переход, если не выполняется условие продолжения
            var endLabel = GetEmptyLabeledNode();
            gotoEnd.TargetLabel = endLabel.Label;
        }

        public override void VisitPrintNode(PrintNode pr)
        {
            TACNodes.Print print = null;
            foreach (var expr in pr.ExprList.ExprList)
            {
                print = new TACNodes.Print
                {
                    Data = RecAssign(expr),
                    Sep = " "
                };
                code.AddNode(print);
            }

            if (print != null)
                print.Sep = Environment.NewLine;
        }

        public override void VisitIfNode(IfNode iif)
        {
            var ifGoto = new TACNodes.IfGoto();

            // Результат вычисления логического выражения
            var cond1 = RecAssign(iif.Conditon);
            ifGoto.Condition = cond1;

            code.AddNode(ifGoto);

            // Разбор тела else (если есть)
            iif.ElseClause?.Visit(this);

            // Пропускаем тело if
            var elseGoTo = new TACNodes.Goto();
            code.AddNode(elseGoTo);

            // Добавление новой метки непосредственно перед телом if
            var newLabelIf = GetEmptyLabeledNode();
            ifGoto.TargetLabel = newLabelIf.Label;

            // Обход выражений тела условного оператора
            iif.IfClause.Visit(this);

            // Метка после тела if, на нее передается управление из else
            var newLabelElse = GetEmptyLabeledNode();
            elseGoTo.TargetLabel = newLabelElse.Label;
        }

        public override void VisitForNode(ForNode f)
        {
            // Значение счетчика цикла и инкремента при инициализации
            var counter = RecAssign(f.Assign.Expr);
            var inc = RecAssign(f.Inc);

            // Метка начала цикла
            var cycle = GetEmptyLabeledNode();

            // Условие выбора направления цикла
            var dirCond = new TACNodes.Assign()
            {
                Result = new TACExpr.Var(),
                Left = inc,
                Right = new TACExpr.IntConst(0),
                Operation = OpCode.GreaterEq
            };
            code.AddNode(dirCond);

            // Выбор направления
            var dirIfGoto = new TACNodes.IfGoto();
            dirIfGoto.Condition = dirCond.Result;
            code.AddNode(dirIfGoto);

            // Для обратного направления счетчик сравнивается с границей на меньше равно - это условие выхода
            var initialBackwardCondition = new TACNodes.Assign
            {
                Result = new TACExpr.Var(),
                Left = counter,
                Right = RecAssign(f.Border),
                Operation = OpCode.LessEq
            };
            code.AddNode(initialBackwardCondition);

            // Добавляем переход за конец цикла при выполнении условия выхода
            var ifGotoBackw = new TACNodes.IfGoto();
            var cond = initialBackwardCondition.Result;
            ifGotoBackw.Condition = cond;
            code.AddNode(ifGotoBackw);

            // Пропускаем ветку прямого направления
            var forwSkipGoto = new TACNodes.Goto();
            code.AddNode(forwSkipGoto);

            // Начало ветки прямого направления
            var forwardLabel = GetEmptyLabeledNode();
            dirIfGoto.TargetLabel = forwardLabel.Label;

            // Для прямого направления счетчик сравнивается с границей на больше равно - это условие выхода
            var initialForwardCondition = new TACNodes.Assign
            {
                Result = new TACExpr.Var(),
                Left = counter,
                Right = RecAssign(f.Border),
                Operation = OpCode.GreaterEq
            };
            code.AddNode(initialForwardCondition);

            // Добавляем переход за конец цикла при выполнении условия выхода
            var ifGotoForw = new TACNodes.IfGoto();
            ifGotoForw.Condition = initialForwardCondition.Result;
            code.AddNode(ifGotoForw);

            // Метка перед телом цикла - сюда происходит переход при пропуске ветки прямого обхода
            var forwSkipLabel = GetEmptyLabeledNode();
            forwSkipGoto.TargetLabel = forwSkipLabel.Label;

            // Дальнейший разбор тела выражения
            f.Body.Visit(this);

            // Пересчет инкремента
            inc = RecAssign(f.Inc);

            // Создаем строку с увеличением счетчика
            var ass1 = new TACNodes.Assign
            {
                Result = counter,
                Left = counter,
                Right = inc,
                Operation = OpCode.Plus
            };
            code.AddNode(ass1);

            // Команда перехода к началу цикла
            var cycleGoto = new TACNodes.Goto { TargetLabel = cycle.Label };
            code.AddNode(cycleGoto);

            // Метка за концом цикла - сюда происходит переход при выполнении условия выхода
            var endCycle = GetEmptyLabeledNode();
            ifGotoForw.TargetLabel = endCycle.Label;
            ifGotoBackw.TargetLabel = endCycle.Label;
        }

        public override void VisitEmptyNode(EmptyNode w)
        {
            code.AddNode(new TACNodes.Empty());
        }

        /// <summary>
        /// Рекурсивный разбор выражений и генерация их кода
        /// </summary>
        private TACExpr.Var RecAssign(ExprNode ex)
        {
            var assign = new TACNodes.Assign();
            var result = new TACExpr.Var();
            assign.Result = result;

            // Обход продолжается до тех пор, пока выражение не окажется переменной или константой
            switch (ex)
            {
                case IdNode tmp1:
                    assign.Left = null;
                assign.Right = GetVarByName(tmp1.Name);
                assign.Operation = OpCode.Copy;
                break;

                case IntNumNode tmp2:
                    assign.Left = null;
                assign.Right = GetConst(tmp2.Num);
                assign.Operation = OpCode.Copy;
                break;

                case BinaryNode tmp3:
                    assign.Left = RecAssign(tmp3.Left);
                assign.Right = RecAssign(tmp3.Right);
                assign.Operation = ConvertOp(tmp3.Operation);
                break;

                case UnaryNode tmp4:
                    assign.Left = null;
                assign.Right = RecAssign(tmp4.Num);
                assign.Operation = ConvertOp(tmp4.Operation);
                break;
            }

            code.AddNode(assign);
            return result;
        }

        /// <summary>
        ///     Конвертер типа операции из АСТ в тип операции ТА кода
        /// </summary>
        private OpCode ConvertOp(OperationType op)
        {
            switch (op)
            {
                case OperationType.Plus:
                    return OpCode.Plus;

                case OperationType.Minus:
                    return OpCode.Minus;

                case OperationType.Mul:
                    return OpCode.Mul;

                case OperationType.Div:
                    return OpCode.Div;

                case OperationType.Greater:
                    return OpCode.Greater;

                case OperationType.Less:
                    return OpCode.Less;

                case OperationType.GreaterEq:
                    return OpCode.GreaterEq;

                case OperationType.LessEq:
                    return OpCode.LessEq;

                case OperationType.Equal:
                    return OpCode.Equal;

                case OperationType.NotEqual:
                    return OpCode.NotEqual;

                case OperationType.Not:
                    return OpCode.Not;

                case OperationType.UnaryMinus:
                    return OpCode.UnaryMinus;

                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }


        private TACNodes.Empty GetEmptyLabeledNode(String name = "")
        {
            var labeledNop = new TACNodes.Empty { IsLabeled = true };
            code.AddNode(labeledNop);
            if (name == "")
                TACodeNameManager.Instance.Label(labeledNop.Label);
            else
                TACodeNameManager.Instance.Name(labeledNop.Label, name);
            return labeledNop;
        }

        /// <summary>
        ///     Найти переменную по имени в исходном коде
        /// </summary>
        private TACExpr.Var GetVarByName(string name)
        {
            if (!m_varsInCode.ContainsKey(name))
                m_varsInCode.Add(name, new TACExpr.Var(name));

            return m_varsInCode[name];
        }

        /// <summary>
        ///     Создать константу
        /// </summary>
        private TACExpr.IntConst GetConst(int value)
        {
            return new TACExpr.IntConst(value);
        }

        public override void VisitIdNode(IdNode id)
        {
            throw new NotImplementedException();
        }

        public override void VisitIntNumNode(IntNumNode num)
        {
            throw new NotImplementedException();
        }

        public override void VisitExprNode(ExprNode node)
        {
            throw new NotImplementedException();
        }
    }
}