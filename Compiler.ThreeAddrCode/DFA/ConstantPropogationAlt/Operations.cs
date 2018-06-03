using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ThreeAddrCode.DFA.ConstantPropogationAlt
{
    public class VarInfo
    {
        public VarInfo(Flag type = Flag.UNDEF, int? value = null)
        {
            Value = value;
            Type = type;
        }

        public enum Flag
        {
            UNDEF, Const, NAC
        }

        public int? Value { get; }
        public Flag Type { get; }

        public override bool Equals(object obj)
        {
            if(obj is VarInfo i)
            {
                if (i.Type != Type) return false;
                return i.Value == Value;
            }
            return false;
        }

        public int? Compare(VarInfo other)
        {
            if (Type == other.Type)
            {
                if (Type == Flag.Const)
                    if (Value == other.Value) return 0;
                    else return null;
                return 0;
            }
            if (Type == Flag.UNDEF) return 1;
            if (Type == Flag.NAC) return -1;
            if (other.Type == Flag.NAC) return 1;
            return -1;
        }

        public VarInfo Merge(VarInfo other)
        {
            if(other.Type == Type)
            {
                if (other.Value != Value) return new VarInfo(Flag.NAC);
                return this;
            }
            if (IsUndef && other.IsConst)
                return other.Copy();
            if (IsUndef && other.IsNac)
                return other.Copy();
            if (IsNac)
                return this.Copy();
            if (other.IsNac)
                return other.Copy();
            return this.Copy();
        }

        public bool IsNac => Type == Flag.NAC;
        public bool IsUndef => Type == Flag.UNDEF;
        public bool IsConst => Type == Flag.Const;

        public override string ToString()
        {
            if (IsNac) return "NAC";
            if (IsUndef) return "UNDEF";
            if (IsConst) return $"Const({Value})";
            return "";
        }

        public static VarInfo MergeBin(VarInfo vi1, VarInfo vi2, OpCode? op)
        {
            if (vi1.IsConst && vi2.IsConst)
            {
                Func<int, int, int> f = (x, y) => x + y;

                if (op == OpCode.Div)
                    f = (x, y) => x / y;
                if (op == OpCode.Equal)
                    f = (x, y) => x == y ? 1 : 0;
                if (op == OpCode.Greater)
                    f = (x, y) => x > y ? 1 : 0;
                if (op == OpCode.GreaterEq)
                    f = (x, y) => x >= y ? 1 : 0;
                if (op == OpCode.Less)
                    f = (x, y) => x < y ? 1 : 0;
                if (op == OpCode.LessEq)
                    f = (x, y) => x <= y ? 1 : 0;
                if (op == OpCode.Minus)
                    f = (x, y) => x - y;
                if (op == OpCode.Mul)
                    f = (x, y) => x * y;
                if (op == OpCode.Plus)
                    f = (x, y) => x + y;

                return new VarInfo(Flag.Const, f(vi1.Value.Value, vi2.Value.Value));
            }
            if (vi1.IsNac || vi2.IsNac) return new VarInfo(Flag.NAC);
            return new VarInfo(Flag.UNDEF);
        }

        public static VarInfo FromAssign(Assign s, VarInfoTable t)
        {
            var r = s.Result;
            if(s.Left is null)
            {
                var v = s.Right;
                if (v is IntConst c) return new VarInfo(Flag.Const, c.Num);
                if (v is Var vr) return t[vr.Id].Copy();
                return new VarInfo(Flag.NAC);
            }
            var v1 = s.Right;
            var v2 = s.Left;

            {
                if (v1 is IntConst c1 && v2 is IntConst c2)
                    return new VarInfo(Flag.Const, c1.Num + c2.Num);
            }

            {
                if (v1 is Var vr1 && v2 is IntConst c2)
                {
                    if (t[vr1.Id].IsConst) return new VarInfo(Flag.Const, t[vr1.Id].Value.Value + c2.Num);
                    if (t[vr1.Id].IsNac) return new VarInfo(Flag.NAC);
                    if (t[vr1.Id].IsUndef) return new VarInfo(Flag.UNDEF);
                }
            }

            {
                if (v2 is Var vr1 && v1 is IntConst c2)
                {
                    if (t[vr1.Id].IsConst) return new VarInfo(Flag.Const, t[vr1.Id].Value.Value + c2.Num);
                    if (t[vr1.Id].IsNac) return new VarInfo(Flag.NAC);
                    if (t[vr1.Id].IsUndef) return new VarInfo(Flag.UNDEF);
                }
            }

            {
                if (v1 is Var vr1 && v2 is Var vr2)
                    return MergeBin(t[vr1.Id], t[vr2.Id], s.Operation);
            }
            throw new Exception();
        }

        public VarInfo Copy() => new VarInfo(Type, Value);

    }

    public class VarInfoTable : Dictionary<Guid, VarInfo>
    {
        private readonly TACode code;

        public VarInfoTable(TACode code)
        {
            foreach( var line in code.CodeList.OfType<Assign>())
            {
                var v = line.Result;
                base[v.Id] = new VarInfo();
            }

            this.code = code;
        }

        public VarInfoTable Merge(VarInfoTable other)
        {
            var res = new VarInfoTable(code);
            foreach(var kv in this)
                res[kv.Key] = this[kv.Key].Merge(other[kv.Key]);
            return res;
        }

        public static bool AreEqual(VarInfoTable a, VarInfoTable b)
        {
            foreach (var kv in a)
                if (!a[kv.Key].Equals(b[kv.Key])) return false;
            return true;
        }

        public VarInfoTable Copy()
        {
            var res = new VarInfoTable(code);
            foreach (var kv in this)
                res[kv.Key] = this[kv.Key];
            return res;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var kv in this)
            {
                sb.AppendLine("     " + TACodeNameManager.Instance[kv.Key] + ":" + kv.Value);
            }
            return sb.ToString();
        }
    }

    public class Operations : ILatticeOperations<VarInfoTable>
    {
        public Operations(TACode code)
        {
            Lower = new VarInfoTable(code);
            var keys = Lower.Keys.ToArray();
            foreach (var k in keys)
                Lower[k] = new VarInfo(VarInfo.Flag.NAC);

            Upper = new VarInfoTable(code);
            foreach (var k in keys)
                Upper[k] = new VarInfo(VarInfo.Flag.UNDEF);
        }

        public VarInfoTable Lower
        { get; }

        public VarInfoTable Upper { get; }

        public bool? Compare(VarInfoTable a, VarInfoTable b)
        {
            throw new NotImplementedException();
        }

        public VarInfoTable Operator(VarInfoTable a, VarInfoTable b)
        {
            return a.Merge(b);
        }
    }

    public class TransferFunction : ITransferFunction<VarInfoTable>
    {
        private readonly TACode _code;

        public TransferFunction(TACode code) {
            _code = code;
        }

        public VarInfoTable Transfer(BasicBlock basicBlock, VarInfoTable input, ILatticeOperations<VarInfoTable> ops)
        {
            var table = input.Copy();
            VarInfo Get(Expr e)
            {
                if (e is IntConst c) return new VarInfo(VarInfo.Flag.Const, c.Num);
                if (e is Var v) return table[v.Id].Copy();
                return new VarInfo(VarInfo.Flag.NAC);
            }

            foreach (var line in basicBlock.CodeList.OfType<Assign>())
            {
                if (line.Left is null) table[line.Result.Id] = Get(line.Right);
                else
                {
                    var vi1 = Get(line.Right);
                    var vi2 = Get(line.Left);

                    table[line.Result.Id] = VarInfo.MergeBin(vi1, vi2, line.Operation);
                }

            }
            return table;
        }
    }

    public class IterativeAlgorithm : GenericIterativeAlgorithm<VarInfoTable>
    {
        public IterativeAlgorithm(ILatticeOperations<VarInfoTable> ops)
        {
            base.Finish = (x, y) => VarInfoTable.AreEqual(x.Item2, y.Item2);
            base.Fill = () => (ops.Upper, ops.Upper);
        }
    }
}
