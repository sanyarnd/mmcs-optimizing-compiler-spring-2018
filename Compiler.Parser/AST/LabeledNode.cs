using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class LabeledNode : StatementNode
    {
        public StatementNode Stat { get; set; }
        public IdNode Label { get; set; }

        public LabeledNode(IdNode label, StatementNode stat)
        {
            Label = label;
            Stat = stat;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitLabeledNode(this);
        }
    }
}