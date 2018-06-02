using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.DFA.ConstantPropagation
{
    public class VarValue
    {
        public enum Type { UNDEF, CONST, NAC };

        /// <summary>
        ///     Операнд определяющий тип переменной
        /// </summary>
        public Type varType;
        /// <summary>
        ///     Операнд со значением константы, если тип переменной CONST
        /// </summary>
        public IntConst value;

        public VarValue()
        {
            varType = Type.UNDEF;
            value = null;
        }

        public VarValue(IntConst c)
        {
            varType = Type.CONST;
            value = c;
        }

        public VarValue(Var v)
        {
            varType = Type.NAC;
            value = null;
        }

        public VarValue CollectionOperator(VarValue right)
        {
            if (right == null || this == right)
                return this;
            if (this.varType == Type.NAC || right.varType == Type.NAC)
                return new VarValue(new Var());
            if (this.varType == Type.CONST && right.varType == Type.CONST)
                return new VarValue(new Var());
            if (this.varType == Type.CONST)
                return this;
            return right;
        }

        public static VarValue UseOperation(VarValue left, VarValue right, OpCode code)
        {
            if (left.varType == Type.CONST && right.varType == Type.CONST)
                return new VarValue(ApplyOperation(left.value, right.value, code));
            return OperationUnderNotConst(left, right);
        }

        private static VarValue OperationUnderNotConst(VarValue left, VarValue right)
        {
            if (left.varType == Type.NAC || right.varType == Type.NAC)
                return new VarValue(new Var());
            if (left.varType == Type.UNDEF || right.varType == Type.UNDEF)
                return new VarValue();
            return null;
        }

        private static IntConst ApplyOperation(IntConst left, IntConst right, OpCode op)
        {
            switch (op)
            {
                case OpCode.Plus: return left + right;
                case OpCode.Minus: return left - right;
                case OpCode.Mul: return left * right;
                case OpCode.Div: return left / right;
                default: return left;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            if (varType == ((VarValue)obj).varType)
            {
                if (varType == Type.CONST && value != ((VarValue)obj).value)
                    return false;
                return true;
            }
            return false;
        }

        public static bool operator ==(VarValue left, VarValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VarValue left, VarValue right)
        {
            return !(Equals(left, right));
        }

        public static bool operator <=(VarValue left, VarValue right)
        {
            if (left.varType <= right.varType)
                return true;
            return false;
        }

        public static bool operator >=(VarValue left, VarValue right)
        {
            if (right.varType <= left.varType)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return value.Num;
        }
    }
}