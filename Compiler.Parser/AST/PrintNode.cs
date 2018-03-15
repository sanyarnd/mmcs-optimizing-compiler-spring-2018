using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }

        public PrintNode(ExprListNode exprlist)
        {
            ExprList = exprlist;
        }

        public override void Visit(IVisitor v)
        {
            v.VisitPrintNode(this);
        }
    }
}