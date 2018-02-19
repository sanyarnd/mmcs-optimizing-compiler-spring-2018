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
        public override void VisitLabelNode(LabelNode l)
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
            c.Expr.Visit(this);
            c.Stat.Visit(this);
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
            foreach (ExprNode ex in exn.ExpList)
                ex.Visit(this);
        }
        public override void VisitIfNode(IfNode iif)
        {
            iif.Expr.Visit(this);
            iif.Stat1.Visit(this);
            iif.Stat2?.Visit(this);
        }
        public override void VisitForNode(ForNode f)
        {
            f.Assign.Visit(this);
            f.Cond.Visit(this);
            f.Inc?.Visit(this);
            f.Stat.Visit(this);
        }
    }
}
