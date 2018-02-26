using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.Optimizations
{
    public class AlgebraicOptimization : IOptimization
    {
        public List<Node> Optimize(List<Node> nodes)
        {
            foreach (Assign node in nodes
                .Where(x => x is Assign assn 
                    && assn.Operation != OpCode.Copy 
                    && assn.Left != null
                )) 
            {
                void SetLeft(int? value=null)
                {
                    node.Operation = OpCode.Copy;
                    node.Left = value.HasValue ? new IntConst(value.Value) : node.Right;
                    node.Right = null;
                }
                void SetRight()
                {
                    node.Operation = OpCode.Copy;
                    node.Right = null;
                }
                switch (node.Operation)
                {
                    case OpCode.Plus:
                        {
                            if (node.Left.Equals(0))
                                SetLeft();
                            else if (node.Right.Equals(0))
                                SetRight();
                            break;
                        }
                    case OpCode.Minus:
                        {
                            if (node.Left.Equals(node.Right))
                                SetLeft(0);
                            else if (node.Right.Equals(0))
                                SetRight();
                            break;
                        }
                    case OpCode.Mul:
                        {
                            if (node.Left.Equals(1))
                                SetLeft();
                            else if (node.Right.Equals(1))
                                SetRight();
                            else if (node.Left.Equals(0) || node.Right.Equals(0))
                                SetLeft(0);
                            break;
                        }
                    case OpCode.Div:
                        {
                            if (node.Right.Equals(1))
                                SetRight();
                            else if (node.Right.Equals(node.Left))
                                SetLeft(1);
                        }
                        break;
                }
            }
            return nodes;
        }
    }
}
