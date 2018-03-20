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
            applied = false;
            var nodes = inputNodes.OfType<Assign>()
                .Where(assn => assn.Operation != OpCode.Copy && assn.Left != null);
            
            foreach(var node in nodes)
            {
                var leftNode = FindOrInitializeExpressionNode(node.Left, out bool leftNodeInitialized, out ExpressionNode leftParentNode);
                var rightNode = FindOrInitializeExpressionNode(node.Right, out bool rightNodeInitialized, out ExpressionNode rightParentNode);
                if (!leftNodeInitialized && !rightNodeInitialized)
                {
                    if (leftParentNode == rightParentNode)
                    {
                        // Если обе ноды имеют общего родителя node.Result идет в AssigneeList.
                        leftParentNode.AssigneeList.Add(node.Result);
                        applied = true;
                    }
                    else {
                        // Если такого нет, надо создать новый ExpressionNode:
                        var resultNode = new ExpressionNode(node.Result);
                        resultNode.LeftNode = leftNode;
                        resultNode.RightNode = rightNode;
                        resultNode.OpCode = node.Operation;
                        exprTreeRoots.Add(resultNode);
                    }
                }
            }

            // Тут мы применяем оптимизацию, обходя дерево и строя новые выражения
            return RecoveryThreeAddrCode();
        }

        private ExpressionNode FindOrInitializeExpressionNode(Expr expr, out bool initializedNewNode, out ExpressionNode parentNode)
        {
            // 1. Ищем дерево, в котором содержится данный expr
            var root = SeekRootTree();
            parentNode = null;
            // 2. Если дерево не нашлось, создаем новый ExpressionNode с expr в качестве листа дерева и initializedNewNode = true
            if (root == null)
            {
                initializedNewNode = true;
                return new ExpressionNode(expr);
            }

            // 3. Если дерево нашлось, запускаем поиск в ширину от корня, чтобы найти самое актуальное вхождение переменной в дереве
            var lastExpression = BFS(root, expr, out parentNode);

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
        private ExpressionNode BFS(ExpressionNode root, Expr expr, out ExpressionNode parent)
        {
            Queue<ExpressionNode> q = new Queue<ExpressionNode>();
            q.Enqueue(root);

            while (q.Count != 0)
            {
                ExpressionNode u = q.Peek();
                parent = u;
                q.Dequeue();
                if (u.LeftNode.AssigneeList.Contains(expr)) {
                    return u.LeftNode;
                }
                q.Enqueue(u.LeftNode);
                if (u.RightNode.AssigneeList.Contains(expr))
                {
                    return u.RightNode;
                }
                q.Enqueue(u.RightNode);
            }
            parent = null;
            return null;
        }

        // Обход готового дерева и построение новых подвыражений
        private List<Node> RecoveryThreeAddrCode() {
            List<Node> nodes = new List<Node>();
            foreach (var expNode in exprTreeRoots) {
                if (expNode.AssigneeList.First() is Var a)
                {
                    Assign node = new Assign();
                    node.Result = a;
                    node.Left = expNode.LeftNode.AssigneeList.First();
                    node.Right = expNode.RightNode.AssigneeList.First();
                    node.Operation = expNode.OpCode;
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }
}
