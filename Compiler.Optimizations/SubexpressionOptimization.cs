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
    public class SubexpressionOptimization : IOptimization
    {
        private List<ExpressionTree> exprForest;
        private ExpressionTree currentTree;

        public List<Node> Optimize(List<Node> inputNodes, out bool applied)
        {
            exprForest = new List<ExpressionTree>();
            var nodes = inputNodes.OfType<Assign>()
                .Where(assn => assn.Operation != OpCode.Copy && assn.Left != null);
            
            foreach(var node in nodes)
            {
                currentTree = null;
                var leftNode = FindOrInitializeExpressionNode(node.Left, out ExpressionNode leftParentNode);
                var rightNode = FindOrInitializeExpressionNode(node.Right, out ExpressionNode rightParentNode);
                if (leftParentNode == null && rightParentNode == null)
                {
                    // создать новый ExpressionNode:
                    var resultNode = new ExpressionNode(node.Result);
                    resultNode.LeftNode = leftNode;
                    resultNode.RightNode = rightNode;
                    resultNode.OpCode = node.Operation;
                    currentTree.AddNode(resultNode);
                }
                else
                {
                    leftParentNode.AssigneeList.Add(node.Result);
                    applied = true;
                }
            }
            // Тут мы применяем оптимизацию, обходя дерево и строя новые выражения
            applied = false;
            return RecoveryThreeAddrCode(inputNodes);
        }

        private ExpressionNode FindOrInitializeExpressionNode(Expr expr, out ExpressionNode parentNode)
        {
            // 1. Ищем дерево, в котором содержится данный expr
            var root = SeekRootTree(expr);
            parentNode = null;
            // 2. Если дерево не нашлось, создаем новый ExpressionNode с expr в качестве листа дерева и initializedNewNode = true
            if (root == null)
            {
                ExpressionNode newNode = new ExpressionNode(expr);
                if (currentTree == null)
                {
                    currentTree = new ExpressionTree();
                    currentTree.AddNode(newNode);
                    exprForest.Add(currentTree);
                }
                return newNode;
            }
                
            
            // 3. Если дерево нашлось, запускаем поиск в ширину от корня, чтобы найти самое актуальное вхождение переменной в дереве
            var lastExpression = BFS(root, expr, out parentNode);
            if (lastExpression == null)
                return new ExpressionNode(expr);

            // 4. Возвращаем найденное вхождение
            return lastExpression;
        }

        private ExpressionNode SeekRootTree(Expr expr)
        {
            foreach (var expTree in exprForest)
                if (expTree.AllAssignees.Contains(expr)) {
                    currentTree = expTree;
                    return expTree.Nodes.Last();
                }
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

                if (u.AssigneeList.Contains(expr))
                {
                    parent = null;
                    return u;
                }

                if (u.LeftNode.AssigneeList.Contains(expr)) {
                    return u.LeftNode;
                }

                if (!u.LeftNode.IsList())
                    q.Enqueue(u.LeftNode);

                if (u.RightNode.AssigneeList.Contains(expr))
                {
                    return u.RightNode;
                }

                if (!u.RightNode.IsList())
                    q.Enqueue(u.RightNode);
            }
            parent = null;
            return null;
        }

        // Обход готового дерева и построение новых подвыражений
        private List<Node> RecoveryThreeAddrCode(List<Node> inputNodes) {
            foreach (var trees in exprForest) { 
                foreach (var expNode in trees.Nodes) {
                    if (!expNode.IsList()) {
                        var node = (Assign)inputNodes.Find(x => expNode.AssigneeList.Contains((x as Assign).Result));
                        if (node != null) {
                            if (expNode.AssigneeList.Count > 1)
                            {
                                expNode.AssigneeList.RemoveAt(0);
                                foreach (var optExpr in expNode.AssigneeList)
                                {
                                    var extraNode = (Assign)inputNodes.Find(x => (x as Assign).Result == optExpr as Var);
                                    extraNode.Left = null;
                                    extraNode.Right = node.Result;
                                    extraNode.Operation = OpCode.Copy;
                                    
                                }
                            }
                        }
                    }
                }
            }
            return inputNodes;
        }
    }
}
