using System.Collections.Generic;
using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class ExprListNode : Node
    {
        public List<ExprNode> ExprList { get; set; }

        public ExprListNode(ExprNode expr)
        {
            ExprList = new List<ExprNode>();
            ExprList.Add(expr);
        }

        public void Add(ExprNode expr)
        {
            ExprList.Add(expr);
        }

        public override void Visit(IVisitor v)
        {
            v.VisitExprListNode(this);
        }
    }
}