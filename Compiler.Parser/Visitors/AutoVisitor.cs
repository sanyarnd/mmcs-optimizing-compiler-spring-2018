using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser.AST;

namespace Compiler.Parser.Visitors
{
    // базовая логика обхода без действий
    // Если нужны действия или другая логика обхода, то соответствующие методы надо переопределять
    // При переопределении методов для задания действий необходимо не забывать обходить подузлы
    public class AutoVisitor : Visitor
    {
        public override void VisitBinaryNode(BinaryNode binop)
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }
        public override void VisitUnaryNode(UnaryNode unop)
        {
            unop.Num.Visit(this);
        }
        public override void VisitLabeledNode(LabeledNode l)
        {
            l.Label.Visit(this);
            l.Stat.Visit(this);
        }
        public override void VisitGoToNode(GoToNode g)
        {
            g.Label.Visit(this);
        }
        public override void VisitAssignNode(AssignNode a)
        {
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }
        public override void VisitCycleNode(CycleNode c)
        {
            c.Condition.Visit(this);
            c.Body.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            foreach (var st in bl.StList)
                st.Visit(this);
        }
        public override void VisitPrintNode(PrintNode pr)
        {
            pr.ExprList.Visit(this);
        }
        public override void VisitExprListNode(ExprListNode exn)
        {
            foreach (ExprNode ex in exn.ExprList)
                ex.Visit(this);
        }
        public override void VisitIfNode(IfNode iif)
        {
            iif.Conditon.Visit(this);
            iif.IfClause.Visit(this);
            iif.ElseClause?.Visit(this);
        }
        public override void VisitForNode(ForNode f)
        {
            f.Assign.Visit(this);
            f.Border.Visit(this);
            f.Inc?.Visit(this);
            f.Body.Visit(this);
        }
    }
}
