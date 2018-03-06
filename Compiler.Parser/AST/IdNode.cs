using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class IdNode : ExprNode
    {
        public string Name { get; set; }

        public IdNode(string name)
        {
            Name = name;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitIdNode(this);
        }
    }
}