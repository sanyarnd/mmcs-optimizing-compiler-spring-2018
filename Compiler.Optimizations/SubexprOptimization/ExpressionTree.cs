using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode;

namespace Compiler.Optimizations.SubexprOptimization
{
    class ExpressionTree
    {
        /// <summary>
        ///     Ноды дерева
        /// </summary>
        public List<ExpressionNode> Nodes { get; set; }

        /// <summary>
        ///     Список всех присвоений в данном дереве
        /// </summary>
        public List<Expr> AllAssignees { get; set; }

        public ExpressionTree()
        {
            Nodes = new List<ExpressionNode>();
            AllAssignees = new List<Expr>();
        }

        public void AddNode(ExpressionNode node)
        {
            Nodes.Add(node);
            AllAssignees = AllAssignees.Concat(node.AssigneeList).Distinct().ToList();
        }
    }
}
