using System.Collections.Generic;
using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList { get; set; }

        public BlockNode(StatementNode stat)
        {
            StList = new List<StatementNode>();
            StList.Add(stat);
        }

        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }

        public override void Visit(IVisitor v)
        {
            v.VisitBlockNode(this);
        }
    }
}