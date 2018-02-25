using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser.AST;


namespace Compiler.Parser.Visitors
{
    /*
     * visitor interface:
            +public virtual void VisitIdNode(IdNode id) { }
            +public virtual void VisitIntNumNode(IntNumNode num) { }
            +public virtual void VisitBinaryNode(BinaryNode binop) { }
            +public virtual void VisitUnaryNode(UnaryNode unop) { }
            +public virtual void VisitAssignNode(AssignNode a) { }
            +public virtual void VisitCycleNode(CycleNode c) { }
            +public virtual void VisitBlockNode(BlockNode bl) { }
            +public virtual void VisitPrintNode(PrintNode p) { }

            +public virtual void VisitExprListNode(ExprListNode el) { }
            +public virtual void VisitLabelNode(LabelNode l) { }
            +public virtual void VisitGoToNode(GoToNode g) { }
            +public virtual void VisitIfNode(IfNode iif) { }
            -public virtual void VisitForNode(ForNode w) { }
            -public virtual void VisitEmptyNode(EmptyNode w) { }
            +public virtual void VisitStatementNode(StatementNode s) { }
            -public virtual void VisitExprNode(ExprNode s) { }
     */
    class PrettyPrintVisitor : Visitor
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
        public override void VisitBinaryNode(BinaryNode binop)
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + binop.Operation + " ";
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode a)
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
        }
        public override void VisitCycleNode(CycleNode c)
        {
            Text += IndentStr() + "while(";
            c.Stat.Visit(this);
            Text += IndentStr() + ")";
            Text += Environment.NewLine;
            c.Expr.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count > 0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                Text += ';';
                if (!(bl.StList[i] is EmptyNode))
                    Text += Environment.NewLine;
                bl.StList[i].Visit(this);
            }
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "}";
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
            Text += ";" + Environment.NewLine;
        }
        public override void VisitLabelNode(LabelNode l)
        {
            l.Label.Visit(this);
            Text += ": {";
            l.Stat.Visit(this);
            Text += "}" + Environment.NewLine;
        }
        public override void VisitExprListNode(ExprListNode el) {
            el.ExpList.ForEach(expr => expr.Visit(this));
        }
        public override void VisitIfNode(IfNode iif)
        {
            Text += IndentStr() + "if (";
            iif.Expr.Visit(this);
            Text += ")" + Environment.NewLine;
            IndentPlus();
            iif.Stat1.Visit(this);
            IndentMinus();
            if (iif.Stat2 != null)
            {
                Text += " else " ;
                iif.Stat2.Visit(this);
            }
        }
        public override void VisitForNode(ForNode w) {
            Text += IndentStr() + "if (";
        }
    }
}
