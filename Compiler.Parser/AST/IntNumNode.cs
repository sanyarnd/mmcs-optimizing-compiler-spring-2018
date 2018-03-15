using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }

        public IntNumNode(int num)
        {
            Num = num;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitIntNumNode(this);
        }
    }
}