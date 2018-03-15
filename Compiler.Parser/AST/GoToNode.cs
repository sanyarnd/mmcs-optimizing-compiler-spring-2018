using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class GoToNode : StatementNode
    {
        public IdNode Label { get; set; }

        public GoToNode(IdNode label)
        {
            Label = label;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitGoToNode(this);
        }
    }
}