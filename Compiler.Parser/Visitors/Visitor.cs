using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser.AST;

namespace Compiler.Parser.Visitors
{
    public abstract class Visitor
    {
        public virtual void VisitIdNode(IdNode id) { }
        public virtual void VisitIntNumNode(IntNumNode num) { }
        public virtual void VisitBinaryNode(BinaryNode binop) { }
        public virtual void VisitUnaryNode(UnaryNode unop) { }
        public virtual void VisitLabeledNode(LabeledNode l) { }
        public virtual void VisitGoToNode(GoToNode g) { }
        public virtual void VisitAssignNode(AssignNode a) { }
        public virtual void VisitCycleNode(CycleNode c) { }
        public virtual void VisitBlockNode(BlockNode bl) { }
        public virtual void VisitPrintNode(PrintNode p) { }
        public virtual void VisitExprNode(ExprNode s) { }
        public virtual void VisitExprListNode(ExprListNode el) { }
        public virtual void VisitIfNode(IfNode iif) { }
        public virtual void VisitForNode(ForNode w) { }
        public virtual void VisitEmptyNode(EmptyNode w) { }
        public virtual void VisitStatementNode(StatementNode s) { }
    }
}
