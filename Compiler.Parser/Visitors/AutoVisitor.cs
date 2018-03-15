using Compiler.Parser.AST;

namespace Compiler.Parser.Visitors
{
    /// <summary>
    ///     Базовая логика обхода без действий
    ///     <para>Если нужны действия или другая логика обхода, то соответствующие методы надо переопределять</para>
    ///     <para>При переопределении методов для задания действий необходимо не забывать обходить подузлы</para>
    /// </summary>
    public abstract class AutoVisitor : IVisitor
    {
        public virtual void VisitBinaryNode(BinaryNode binop)
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }

        public virtual void VisitUnaryNode(UnaryNode unop)
        {
            unop.Num.Visit(this);
        }

        public virtual void VisitLabeledNode(LabeledNode l)
        {
            l.Label.Visit(this);
            l.Stat.Visit(this);
        }

        public virtual void VisitGoToNode(GoToNode g)
        {
            g.Label.Visit(this);
        }

        public virtual void VisitAssignNode(AssignNode a)
        {
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }

        public virtual void VisitCycleNode(CycleNode c)
        {
            c.Condition.Visit(this);
            c.Body.Visit(this);
        }

        public virtual void VisitBlockNode(BlockNode bl)
        {
            foreach (var st in bl.StList)
                st.Visit(this);
        }

        public virtual void VisitPrintNode(PrintNode pr)
        {
            pr.ExprList.Visit(this);
        }

        public virtual void VisitExprListNode(ExprListNode exn)
        {
            foreach (var ex in exn.ExprList)
                ex.Visit(this);
        }

        public virtual void VisitIfNode(IfNode iif)
        {
            iif.Conditon.Visit(this);
            iif.IfClause.Visit(this);
            iif.ElseClause?.Visit(this);
        }

        public virtual void VisitForNode(ForNode f)
        {
            f.Assign.Visit(this);
            f.Border.Visit(this);
            f.Inc.Visit(this);
            f.Body.Visit(this);
        }

        public abstract void VisitEmptyNode(EmptyNode w);

        public abstract void VisitIdNode(IdNode id);

        public abstract void VisitIntNumNode(IntNumNode num);

        public abstract void VisitExprNode(ExprNode node);
    }
}