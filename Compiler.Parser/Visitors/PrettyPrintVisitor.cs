using System;
using System.Linq;
using Compiler.Parser.AST;


namespace Compiler.Parser.Visitors
{
    /// <summary>
    /// Класс реализующий Pretty-print visitor
    /// Восстанавливает текст программы по AST
    /// </summary>
    public class PrettyPrintVisitor : IVisitor
    {
        /// <summary>
        /// Текст программы.
        /// </summary>
        public string Text = "";
        /// <summary>
        /// Текущий отступ.
        /// </summary>
        private int Indent = 0;

        /// <summary>
        /// Возвращает строку с текущим отступом от ее начала.
        /// </summary>
        /// <returns></returns>
        private string IndentStr()
        {
            return new string(' ', Indent);
        }

        /// <summary>
        /// Увеличения текущего отступа
        /// </summary>
        private void IndentPlus()
        {
            Indent += 2;
        }

        /// <summary>
        /// Уменьшение текущего отступа
        /// </summary>
        private void IndentMinus()
        {
            Indent -= 2;
        }

        /// <summary>
        /// Посещение IdNode
        /// </summary>
        /// <param name="id">Узел IdNode</param>
        public virtual void VisitIdNode(IdNode id)
        {
            Text += id.Name;
        }

        /// <summary>
        /// Посещение IntNumNode
        /// </summary>
        /// <param name="num">Узел IntNumNode</param>
        public virtual void VisitIntNumNode(IntNumNode num)
        {
            Text += num.Num.ToString();
        }

        /// <summary>
        /// Посещение узла с унарной операцией
        /// </summary>
        /// <param name="unop">Узел UnaryNode</param>
        public virtual void VisitUnaryNode(UnaryNode unop) {
            Text += unop.Operation.ToSymbolString() + " ";
            unop.Num.Visit(this);
        }

        /// <summary>
        /// Посещение узла с бинарной операцией
        /// </summary>
        /// <param name="binop">Узел BinaryNode</param>
        public virtual void VisitBinaryNode(BinaryNode binop)
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + binop.Operation.ToSymbolString() + " ";
            binop.Right.Visit(this);
            Text += ")";
        }

        /// <summary>
        /// Посещение узла с операцией присвоения
        /// </summary>
        /// <param name="a">Узел AssignNode</param>
        public virtual void VisitAssignNode(AssignNode a)
        {
            if (!Text.EndsWith("for("))
                Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
        }

        /// <summary>
        /// Посещение узла с циклом while
        /// </summary>
        /// <param name="c">Узел CycleNode</param>
        public virtual void VisitCycleNode(CycleNode c)
        {
            Text += IndentStr() + "while(";
            c.Condition.Visit(this);
            Text += ")";
            Text += Environment.NewLine;
            if (!(c.Body is BlockNode))
            {
                IndentPlus();
                c.Body.Visit(this);
                Text += ";" + Environment.NewLine;
                IndentMinus();
            }
            else
                c.Body.Visit(this);
        }

        /// <summary>
        /// Посещение узла блока
        /// </summary>
        /// <param name="bl">Узел BlockNode</param>
        public virtual void VisitBlockNode(BlockNode bl)
        {
            bool isNotBeginOfProgram = Text != "";
            if (isNotBeginOfProgram)
                Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();
            var Count = bl.StList.Count;
            for (var i = 0; i < Count; i++)
            {
                bl.StList[i].Visit(this);
                if (!(bl.StList[i] is EmptyNode))
                    if (!(bl.StList[i] is ForNode  || bl.StList[i] is CycleNode ||
                          bl.StList[i] is LabeledNode || bl.StList[i] is IfNode))
                        Text += ";" + Environment.NewLine;
            }
            IndentMinus();
            if (isNotBeginOfProgram)
                Text += IndentStr() + "}" + Environment.NewLine;
        }

        /// <summary>
        /// Посещение узла с оператором печати
        /// </summary>
        /// <param name="p">Узел PrintNode</param>
        public virtual void VisitPrintNode(PrintNode p)
        {
            Text += IndentStr() + "print(";
            p.ExprList.Visit(this);
            Text += ")";
        }

        /// <summary>
        /// Посещение узла с оператором goto
        /// </summary>
        /// <param name="g">Узел GoToNode</param>
        public virtual void VisitGoToNode(GoToNode g)
        {
            Text += IndentStr() + "goto ";
            g.Label.Visit(this);
        }

        /// <summary>
        /// Посещение узла с меткой
        /// </summary>
        /// <param name="l">Узел LabeledNode</param>
        public virtual void VisitLabeledNode(LabeledNode l)
        {
            Text += IndentStr();
            l.Label.Visit(this);
            Text += ":" + Environment.NewLine;
            if (!(l.Stat is BlockNode))
            {
                IndentPlus();
                l.Stat.Visit(this);
                Text += ";" + Environment.NewLine;
                IndentMinus();
            }
            else
                l.Stat.Visit(this);
        }

        /// <summary>
        /// Посещение узла с меткой
        /// </summary>
        /// <param name="el">Узел LabeledNode</param>
        public virtual void VisitExprListNode(ExprListNode el) {
            var last = el.ExprList.Last();
            el.ExprList.ForEach(expr => 
                {
                    expr.Visit(this);
                    if (expr != last)
                        Text += ",";
                });
        }

        /// <summary>
        /// Посещение узла условного оператора
        /// </summary>
        /// <param name="iif">Узел IfNode</param>
        public virtual void VisitIfNode(IfNode iif)
        {
            Text += IndentStr() + "if (";
            iif.Conditon.Visit(this);
            Text += ")" + Environment.NewLine;
            
            if (!(iif.IfClause is BlockNode))
            {
                IndentPlus();
                iif.IfClause.Visit(this);
                Text += ";" + Environment.NewLine;
                IndentMinus();
            }
            else
                iif.IfClause.Visit(this);
            if (iif.ElseClause != null)
            {
                Text += IndentStr()  + "else" + Environment.NewLine;
                if (!(iif.ElseClause is BlockNode))
                {
                    IndentPlus();
                    iif.ElseClause.Visit(this);
                    Text += ";" + Environment.NewLine;
                    IndentMinus();
                }
                else
                    iif.ElseClause.Visit(this);
            }
        }

        /// <summary>
        /// Посещение узла с циклом for
        /// </summary>
        /// <param name="w">Узел ForNode</param>
        public virtual void VisitForNode(ForNode w)
        {
            Text += IndentStr() + "for(";
            w.Assign.Visit(this);
            Text += ",";
            w.Border.Visit(this);
            if (w.Inc != null)
            {
                Text += ",";
                w.Inc.Visit(this);
            }
            Text += ")";
            Text += Environment.NewLine;
             if (!(w.Body is BlockNode))
            {
                IndentPlus();
                w.Body.Visit(this);
                Text += ";" + Environment.NewLine;
                IndentMinus();
            }
            else
                w.Body.Visit(this);
        }

        /// <summary>
        /// Посещение узла с пустым оператором
        /// </summary>
        /// <param name="w">Узел EmptyNode</param>
        public virtual void VisitEmptyNode(EmptyNode w) {}

        /// <summary>
        /// Посещение узла с выражением
        /// </summary>
        /// <param name="ы">Узел ExprNode</param>
        public virtual void VisitExprNode(ExprNode s)
        {
            s.Visit(this);
        }
    }
}
