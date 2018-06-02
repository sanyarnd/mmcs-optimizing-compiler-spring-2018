using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;

namespace Compiler.Optimizations
{
    /// <summary>
    ///     Класс для перемещения объявлений как можно ближе к реализации
    ///     Использовать только внутри базового блока
    ///     Перед использованием удалить неиспользуемый код
    /// </summary>
    public class DeclarationOptimization : IOptimization
    {
        public List<Node> Optimize(List<Node> inputNodes, out bool applied)
        {
            bool app = false;
            var currentVariables = new List<Guid>();
            var usedVariables = new List<Guid>();
            var nodes = inputNodes;

            for (int currentIndex = nodes.Count - 1; currentIndex >= 0; currentIndex--)
            {
                if (nodes[currentIndex] is Assign assignCurrent)
                {
                    // If left is variable and didn't use add to List
                    if (assignCurrent.Left is Var currentLeft && !usedVariables.Contains(currentLeft.Id))
                    {
                        currentVariables.Add(currentLeft.Id);
                    }

                    // If right is variable and didn't use add to List
                    if (assignCurrent.Right is Var currentRight && !usedVariables.Contains(currentRight.Id))
                    {
                        currentVariables.Add(currentRight.Id);
                    }

                    // If not variables in "USE" go forward
                    if (currentVariables.Count == 0) { continue; }

                    for (var i = currentIndex - 1; i >= 0; i--)
                    {
                        if (currentVariables.Count == 0)
                        {
                            break;
                        }

                        if (nodes[i] is Assign iAssign)
                        {
                            var id = iAssign.Result.Id;

                            // If current result id in currentVariables, try to move it
                            if (currentVariables.Contains(id) && !iAssign.IsLabeled)
                            {
                                int j = i + 1;
                                while (j < currentIndex)
                                {
                                    try
                                    {
                                        if (nodes[j + 1] is Assign nnAssign && nodes[j] is Assign nAssign)
                                        {
                                            if (nnAssign.Left is Var nnLeft && nnAssign.Right is Var nnRight)
                                            {
                                                if (nnLeft.Id == id && nnRight.Id == nAssign.Result.Id)
                                                {
                                                    break;
                                                }
                                                if (nnLeft.Id == nAssign.Result.Id && nnRight.Id == id)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (nodes[j] is Assign jAssign)
                                    {
                                        // If left or right is Variable, stop moving
                                        if (jAssign.Left is Var || jAssign.Right is Var)
                                        {
                                            if (jAssign.Left is Var jLeft)
                                            {
                                                if (id == jLeft.Id)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (id == (jAssign.Right as Var).Id)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    // else move node
                                    var tmp = nodes[i];
                                    nodes[i] = nodes[j];
                                    nodes[j] = tmp;
                                    j++;
                                    i++;
                                }
                                // remove from currentVariables and add to used variables
                                currentVariables.Remove(id);
                                usedVariables.Add(id);
                            }
                        }
                    }
                }
            }
            applied = app;
            return nodes;
        }
    }
}