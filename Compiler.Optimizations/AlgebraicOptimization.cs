using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.Optimizations
{
    public class AlgebraicOptimization : IOptimization
    {
        private static readonly IntConst Zero = new IntConst(0);
        private static readonly IntConst One = new IntConst(1);

        private bool SetLeft(Assign node, int? value = null)
        {
            node.Operation = OpCode.Copy;
            node.Left = value.HasValue ? new IntConst(value.Value) : node.Right;
            node.Right = null;
            SwapLeftRight(node);
            return true;
        }

        private bool SetRight(Assign node)
        {
            node.Operation = OpCode.Copy;
            node.Right = null;
            SwapLeftRight(node);
            return true;
        }

        private void SwapLeftRight(Assign node)
        {
            var t = node.Left;
            node.Left = node.Right;
            node.Right = t;
        }

        public List<Node> Optimize(List<Node> nodes, out bool applied)
        {
            var app = false;

            var enumerable = nodes
                .OfType<Assign>()
                .Where(assn => assn.Operation != OpCode.Copy && assn.Left != null);
            foreach (var node in enumerable)
            {
                switch (node.Operation)
                {
                    case OpCode.Plus:
                        if (node.Left.Equals(Zero))
                            app = SetLeft(node);
                        else if (node.Right.Equals(Zero))
                            app = SetRight(node);
                        break;

                    case OpCode.Minus:
                        if (node.Left.Equals(node.Right))
                            app = SetLeft(node, 0);
                        else if (node.Right.Equals(Zero))
                            app = SetRight(node);
                        break;

                    case OpCode.Mul:
                        if (node.Left.Equals(One))
                            app = SetLeft(node);
                        else if (node.Right.Equals(One))
                            app = SetRight(node);
                        else if (node.Left.Equals(Zero) || node.Right.Equals(Zero))
                            app = SetLeft(node, 0);
                        break;

                    case OpCode.Div:
                        if (node.Right.Equals(One))
                            app = SetRight(node);
                        else if (node.Right.Equals(node.Left))
                            app = SetLeft(node, 1);
                        break;
                    
                    //default:
                        //throw new ArgumentOutOfRangeException();
                }
            }

            applied = app;
            return nodes;
        }
    }
}