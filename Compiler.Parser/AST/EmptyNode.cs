using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class EmptyNode : StatementNode
    {
        public override void Visit(IVisitor v)
        {
            v.VisitEmptyNode(this);
        }
    }
}