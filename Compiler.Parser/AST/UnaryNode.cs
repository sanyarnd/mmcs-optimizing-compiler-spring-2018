using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class UnaryNode : ExprNode
    {
        public ExprNode Num { get; set; }
        public OperationType Operation { get; set; }

        public UnaryNode(ExprNode num, OperationType op)
        {
            Num = num;
            Operation = op;
        }

        public UnaryNode(int num, OperationType op) : this(new IntNumNode(num), op)
        {
        }

        public override void Visit(IVisitor v)
        {
            v.VisitUnaryNode(this);
        }
    }
}