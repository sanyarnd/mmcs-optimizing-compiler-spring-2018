using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.ILcodeGenerator
{
    public class TAcode2ILcodeTranslator
    {
        private  Dictionary<string, LocalBuilder> vars = new Dictionary<string, LocalBuilder>();
        private  Dictionary<string, Label> labels = new Dictionary<string, Label>();
        private static GenCodeCreator gen;
        public  void Translate(TACode tACode)
        {
            gen = new GenCodeCreator();
            gen.Emit(OpCodes.Nop);
            foreach (var currentInstuction in tACode.CodeList)
            {
                if (currentInstuction.IsLabeled)
                {
                    var curLabel = gen.DefineLabel();
                    labels.Add(currentInstuction.Label.ToString(), curLabel);
                }
            }
            foreach (var currentInstuction in tACode.CodeList)
            {
                if (currentInstuction.IsLabeled)
                {
                    gen.Emit(OpCodes.Nop);
                    gen.MarkLabel(labels[currentInstuction.Label.ToString()]);
                }
                switch (currentInstuction)
                {
                    case Assign assignNode:
                        if (!vars.ContainsKey(assignNode.Result.Id.ToString()))
                            vars.Add(assignNode.Result.Id.ToString(), gen.DeclareLocal(typeof(int)));

                        if (assignNode.Left != null)
                            if (assignNode.Left is Var)
                                gen.Emit(OpCodes.Ldloc, vars[(assignNode.Left as Var).Id.ToString()]);
                            else
                                gen.Emit(OpCodes.Ldc_I4, (assignNode.Left as IntConst).Num);


                        if (assignNode.Right is Var)
                            gen.Emit(OpCodes.Ldloc, vars[(assignNode.Right as Var).Id.ToString()]);
                        else
                            gen.Emit(OpCodes.Ldc_I4, (assignNode.Right as IntConst).Num);


                        switch (assignNode.Operation)
                        {
                            case ThreeAddrCode.OpCode.Plus:
                                if (assignNode.Left != null)
                                    gen.Emit(OpCodes.Add);
                                break;

                            case ThreeAddrCode.OpCode.Minus:
                                if (assignNode.Left != null)
                                    gen.Emit(OpCodes.Sub);
                                else
                                    gen.Emit(OpCodes.Neg);
                                break;

                            case ThreeAddrCode.OpCode.Mul:
                                gen.Emit(OpCodes.Mul);
                                break;

                            case ThreeAddrCode.OpCode.Div:
                                gen.Emit(OpCodes.Div);
                                break;

                            case ThreeAddrCode.OpCode.UnaryMinus:
                                gen.Emit(OpCodes.Neg);
                                break;

                            case ThreeAddrCode.OpCode.Copy:
                                break;

                            case ThreeAddrCode.OpCode.Greater:
                                gen.Emit(OpCodes.Cgt);
                                break;

                            case ThreeAddrCode.OpCode.Less:
                                gen.Emit(OpCodes.Clt);
                                break;

                            case ThreeAddrCode.OpCode.GreaterEq:
                                gen.Emit(OpCodes.Clt);
                                gen.Emit(OpCodes.Ldc_I4_0);
                                gen.Emit(OpCodes.Ceq);
                                break;

                            case ThreeAddrCode.OpCode.LessEq:
                                gen.Emit(OpCodes.Cgt);
                                gen.Emit(OpCodes.Ldc_I4_0);
                                gen.Emit(OpCodes.Ceq);
                                break;

                            case ThreeAddrCode.OpCode.Equal:
                                gen.Emit(OpCodes.Ceq);
                                break;

                            case ThreeAddrCode.OpCode.NotEqual:
                                gen.Emit(OpCodes.Ceq);
                                gen.Emit(OpCodes.Ldc_I4_0);
                                gen.Emit(OpCodes.Ceq);
                                break;

                            case ThreeAddrCode.OpCode.Not:
                                gen.Emit(OpCodes.Ldc_I4_0);
                                gen.Emit(OpCodes.Ceq);
                                break;
                        };
                        gen.Emit(OpCodes.Stloc, vars[assignNode.Result.Id.ToString()]);
                        break;

                    case Empty emptyNode:
                        gen.Emit(OpCodes.Nop);
                        break;

                    case IfGoto ifgotoNode: //Доделать
                        var curLabel = gen.DefineLabel();
                        if (ifgotoNode.Condition is Var)
                            gen.Emit(OpCodes.Ldloc, vars[(ifgotoNode.Condition as Var).Id.ToString()]);
                        else
                            gen.Emit(OpCodes.Ldc_I4, (ifgotoNode.Condition as IntConst).Num);
                        gen.Emit(OpCodes.Brfalse, curLabel);
                        gen.Emit(OpCodes.Br, labels[ifgotoNode.TargetLabel.ToString()]);
                        gen.Emit(OpCodes.Nop);
                        gen.MarkLabel(curLabel);
                        break;

                    case Goto gotoNode:

                        gen.Emit(OpCodes.Br, labels[gotoNode.TargetLabel.ToString()]);
                        break;

                    case Print printNode:
                        if (printNode.Data is Var)
                            gen.Emit(OpCodes.Ldloc, vars[(printNode.Data as Var).Id.ToString()]);
                        else
                            gen.Emit(OpCodes.Ldc_I4, (printNode.Data as IntConst).Num);
                        gen.EmitWriteLine();
                        break;

                    default:
                        throw new Exception("Unknow type of node of TAcode");
                }
            }
            gen.Emit(OpCodes.Ret);
           
        }
        public void RunProgram()
        {
            gen.RunProgram();
        }

        public string PrintCommands()
        {
            string result = "";
            foreach (var s in gen.commands)
                result += s+'\n';//Console.WriteLine(s);
            return result;
        }
    }
}
