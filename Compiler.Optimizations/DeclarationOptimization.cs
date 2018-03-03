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

                            //If current result id in currentVariables, try to move it
                            if (currentVariables.Contains(id))
                            {
                                for (int j = i + 1; j < currentIndex; j++)
                                {
                                    if (nodes[j] is Assign jAssign)
                                    {
                                        //If left or right is Variable, stop moving
                                        if (iAssign.Left is Var || iAssign.Right is Var)
                                        {
                                            if (iAssign.Left is Var iLeft)
                                            {
                                                if (jAssign.Result.Id == iLeft.Id)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (jAssign.Result.Id == (iAssign.Right as Var).Id)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        //else move node
                                        var tmp = nodes[i];
                                        nodes[i] = nodes[j];
                                        nodes[j] = tmp;
                                        i++;
                                    }

                                    //remove from currentVariables and add to used variables
                                    currentVariables.Remove(id);
                                    usedVariables.Add(id);
                                }
                            }
                        }
                    }
                }

                //if (!(nodes[currentIndex] is Assign) || nodes[currentIndex].IsLabeled)
                //    continue;
                //for (int nextIndex = currentIndex + 1; nextIndex < nodes.Count; nextIndex++)
                //{
                //    if (nodes[nextIndex] is Assign)
                //    {
                //        if ((nodes[nextIndex] as Assign).Left == (nodes[currentIndex] as Assign).Result ||
                //            (nodes[nextIndex] as Assign).Right == (nodes[currentIndex] as Assign).Result ||
                //            (nodes[nextIndex] as Assign).Result == (nodes[currentIndex] as Assign).Result ||
                //            (nodes[nextIndex] as Assign).Result == (nodes[currentIndex] as Assign).Left ||
                //            (nodes[nextIndex] as Assign).Result == (nodes[currentIndex] as Assign).Right)
                //        {
                //            nodes.Insert(nextIndex, nodes[currentIndex]);
                //            nodes.RemoveAt(currentIndex);
                //            //if (nextIndex != currentIndex + 1)
                //            //    currentIndex++;
                //            app = true;
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        nodes.Insert(nextIndex, nodes[currentIndex]);
                //        nodes.RemoveAt(currentIndex);
                //        //if (nextIndex != currentIndex + 1)
                //        //    currentIndex++;
                //        app = true;
                //        break;
                //    }
                //}
            }
            applied = app;
            return nodes;
        }
    }
}