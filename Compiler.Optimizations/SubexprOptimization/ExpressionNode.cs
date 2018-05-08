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

        /// <summary>
        ///     Левый операнд
        /// </summary>
        public ExpressionNode LeftNode { get; set; }
        /// <summary>
        ///     Правый операнд
        /// </summary>
        public ExpressionNode RightNode { get; set; }
        /// <summary>
        ///     Производимая операция
        /// </summary>
        public OpCode OpCode { get; set; }

        // Должен содержать хотя бы один элемент
        /// <summary>
        ///     Хранилище операндов
        /// </summary>
        public List<Expr> AssigneeList { get; }
        

        public ExpressionNode(Expr expression)
        {
            AssigneeList = new List<Expr>();
            AssigneeList.Add(expression);
        }

        public bool IsList()
        {
            if (RightNode == null && LeftNode == null)
                return true;
            return false;
        }
    }
}
