using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.DFA.ConstantPropagation
{
    public class TransferFunctions : ITransferFunction<Dictionary<Guid, VarValue>>
    {
        public Dictionary<Guid, VarValue> Transfer(BasicBlock basicBlock, Dictionary<Guid, VarValue> input, ILatticeOperations<Dictionary<Guid, VarValue>> ops)
        {
            Dictionary<Guid, VarValue> res = input;
            foreach (var node in basicBlock.CodeList)
            {
                if (node is Assign)
                {
                    var asNode = node as Assign;
                    if (asNode.Left == null && asNode.Right is IntConst)
                    {
                        res[asNode.Result.Id] = new VarValue(asNode.Right as IntConst);
                    }
                    if (asNode.Left != null && asNode.Right != null)
                    {
                        if ((asNode.Left is IntConst || res[(asNode.Left as Var).Id].varType == VarValue.Type.CONST) &&
                            (asNode.Right is IntConst || res[(asNode.Right as Var).Id].varType == VarValue.Type.CONST))
                        {

                            var left = new VarValue();
                            var right = new VarValue();

                            if (asNode.Left is IntConst)
                            {
                                left = new VarValue(asNode.Left as IntConst);
                            }
                            else
                            {
                                left = res[(asNode.Left as Var).Id];
                            }

                            if (asNode.Right is IntConst)
                            {
                                right = new VarValue(asNode.Right as IntConst);
                            }
                            else
                            {
                                right = res[(asNode.Right as Var).Id];
                            }

                            res[asNode.Result.Id] = VarValue.UseOperation(left, right, asNode.Operation);
                        }
                        else if (asNode.Left is Var && res[(asNode.Left as Var).Id].varType == VarValue.Type.NAC ||
                            asNode.Right is Var && res[(asNode.Right as Var).Id].varType == VarValue.Type.NAC)

                            res[asNode.Result.Id] = new VarValue(asNode.Result);

                        else res[asNode.Result.Id] = new VarValue();
                    }
                }
            }
            return res;
        }
    }
}