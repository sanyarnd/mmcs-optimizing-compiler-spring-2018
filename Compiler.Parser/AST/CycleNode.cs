using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class CycleNode : StatementNode
    {
        public ExprNode Condition { get; set; }
        public StatementNode Body { get; set; }

        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Condition = expr;
            Body = stat;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitCycleNode(this);
        }
    }
}