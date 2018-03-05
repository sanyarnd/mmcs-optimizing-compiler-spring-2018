using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class IfNode : StatementNode
    {
        public ExprNode Conditon { get; set; }
        public StatementNode IfClause { get; set; }
        public StatementNode ElseClause { get; set; }

        public IfNode(ExprNode expr, StatementNode ifClause, StatementNode elseClause = null)
        {
            Conditon = expr;
            IfClause = ifClause;
            ElseClause = elseClause;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitIfNode(this);
        }
    }
}