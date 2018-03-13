using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode;

namespace Compiler.Optimizations.SubexprOptimization
{
    class ExpressionNode
    {
        // Могут быть null для листовых вершин
        public ExpressionNode LeftNode { get; set; }
        public ExpressionNode RightNode { get; set; }
        public OpCode OpCode { get; set; }

        // Должен содержать хотя бы один элемент
        public List<Expr> AssigneeList { get; }
        

        public ExpressionNode(Expr expression)
        {
            AssigneeList = new List<Expr>();
            AssigneeList.Add(expression);
        }
    }
}
