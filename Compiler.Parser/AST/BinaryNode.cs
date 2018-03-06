using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class BinaryNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public OperationType Operation { get; set; }

        public BinaryNode(ExprNode left, ExprNode right, OperationType op)
        {
            Left = left;
            Right = right;
            Operation = op;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitBinaryNode(this);
        }
    }
}