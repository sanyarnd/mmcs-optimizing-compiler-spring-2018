using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser.AST;


namespace Compiler.Parser.Visitors
{
    public class PrettyPrintVisitor : Visitor
    {
        public string Text = "";
        private int Indent = 0;

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 2;
        }
        private void IndentMinus()
        {
            Indent -= 2;
        }
        public override void VisitIdNode(IdNode id)
        {
            Text += id.Name;
        }
        public override void VisitIntNumNode(IntNumNode num)
        {
            Text += num.Num.ToString();
        }
        public override void VisitUnaryNode(UnaryNode unop) {
            Text += unop.Operation.ToString() + " ";
            unop.Num.Visit(this);
        }
        public override void VisitBinaryNode(BinaryNode binop)
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + binop.Operation.ToString() + " ";
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode a)
        {
            if (!Text.EndsWith("for("))
                Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
        }
        public override void VisitCycleNode(CycleNode c)
        {
            Text += IndentStr() + "while(";
            c.Expr.Visit(this);
            Text += ")";
            Text += Environment.NewLine;
            c.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();
            var Count = bl.StList.Count;
            for (var i = 0; i < Count; i++)
            {
                bl.StList[i].Visit(this);
                if (!(bl.StList[i] is EmptyNode))
                    if (!(bl.StList[i] is ForNode  || bl.StList[i] is CycleNode ||
                          bl.StList[i] is LabelNode || bl.StList[i] is IfNode))
                        Text += ";" + Environment.NewLine;
            }
            IndentMinus();
            Text +=  IndentStr() + "}" + Environment.NewLine;
        }
        public override void VisitPrintNode(PrintNode p)
        {
            Text += IndentStr() + "print(";
            p.ExprList.Visit(this);
            Text += ")";
        }
        public override void VisitGoToNode(GoToNode g)
        {
            Text += IndentStr() + "goto ";
            g.Label.Visit(this);
        }
        public override void VisitLabelNode(LabelNode l)
        {
            l.Label.Visit(this);
            Text += ":" + Environment.NewLine;
            l.Stat.Visit(this);
        }
        public override void VisitExprListNode(ExprListNode el) {
            var last = el.ExpList.Last();
            el.ExpList.ForEach(expr => 
                {
                    expr.Visit(this);
                    if (expr != last)
                        Text += ",";
                });
        }
        public override void VisitIfNode(IfNode iif)
        {
            Text += IndentStr() + "if (";
            iif.Expr.Visit(this);
            Text += ")" + Environment.NewLine;
            iif.Stat1.Visit(this);
            if (iif.Stat2 != null)
            {
                Text += IndentStr()  + "else" + Environment.NewLine;
                iif.Stat2.Visit(this);
            }
        }

        public override void VisitForNode(ForNode w)
        {
            Text += IndentStr() + "for(";
            w.Assign.Visit(this);
            Text += ",";
            w.Cond.Visit(this);
            if (w.Inc != null)
            {
                Text += ",";
                w.Inc.Visit(this);
            }
            Text += ")";
            Text += Environment.NewLine;
            w.Stat.Visit(this);
        }
        public override void VisitEmptyNode(EmptyNode w) {}

        public override void VisitExprNode(ExprNode s)
        {
            s.Visit(this);
        }
    }
}
