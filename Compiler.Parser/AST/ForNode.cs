using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class ForNode : StatementNode
    {
        public AssignNode Assign { get; set; }
        public ExprNode Border { get; set; }
        public ExprNode Inc { get; set; }
        public StatementNode Body { get; set; }

        public ForNode(AssignNode assign, ExprNode bord, ExprNode inc, StatementNode body)
        {
            Assign = assign;
            Border = bord;
            Inc = inc;
            Body = body;
        }

        public ForNode(AssignNode assign, ExprNode bord, StatementNode body) : this(assign, bord, new IntNumNode(1), body)
        {
        }

        public override void Visit(IVisitor v)
        {
            v.VisitForNode(this);
        }
    }
}