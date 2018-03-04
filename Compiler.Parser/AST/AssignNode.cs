using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }

        public AssignNode(IdNode id, ExprNode expr)
        {
            Id = id;
            Expr = expr;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitAssignNode(this);
        }
    }
}