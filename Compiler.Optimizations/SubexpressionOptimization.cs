using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Optimizations.SubexprOptimization;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Optimizations
{
    class SubexpressionOptimization : IOptimization
    {
        private List<ExpressionNode> exprTreeRoots;

        public List<Node> Optimize(List<Node> inputNodes, out bool applied)
        {
            var nodes = inputNodes.OfType<Assign>()
                .Where(assn => assn.Operation != OpCode.Copy && assn.Left != null);
            
            foreach(var node in nodes)
            {
                var leftNode = FindOrInitializeExpressionNode(node.Left, out bool leftNodeInitialized);
                var rightNode = FindOrInitializeExpressionNode(node.Right, out bool rightNodeInitialized);
                if (!leftNodeInitialized && !rightNodeInitialized)
                {
                    // Если у нас обе ноды уже были в дереве до этого,
                    // ищем их родителя с тем же OpCode.
                    // Если такой есть, node.Result идет в AssigneeList.
                    
                    // Если такого нет, надо создать новый ExpressionNode:
                    // var resultNode = new ExpressionNode(node.Result);
                    // resultNode.LeftNode = leftNode;
                    // resultNode.RightNode = rightNode;
                    // resultNode.Operation = node.Operation;
                }
            }

            // Тут мы применяем оптимизацию, обходя дерево и строя новые выражения
            applied = false;
            return null;
        }

        private ExpressionNode FindOrInitializeExpressionNode(Expr expr, out bool initializedNewNode)
        {
            // 1. Ищем дерево, в котором содержится данный expr
            var root = SeekRootTree();

            // 2. Если дерево не нашлось, создаем новый ExpressionNode с expr в качестве листа дерева и initializedNewNode = true
            if(root == null)
            {
                initializedNewNode = true;
                return new ExpressionNode(expr);
            }

            // 3. Если дерево нашлось, запускаем поиск в ширину от корня, чтобы найти самое актуальное вхождение переменной в дереве
            var lastExpression = BFS(root);

            // 4. Возвращаем найденное вхождение
            initializedNewNode = false;
            return lastExpression;
        }

        private ExpressionNode SeekRootTree()
        {
            return null;
        }

        // Поиск в ширину внутри дерева выражений
        // Вернет null, если переменная не была найдена в указанном дереве
        private ExpressionNode BFS(ExpressionNode root)
        {

            return null;
        }
    }
}
