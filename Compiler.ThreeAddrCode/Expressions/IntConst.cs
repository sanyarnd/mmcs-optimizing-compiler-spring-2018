using System;
namespace Compiler.ThreeAddrCode.Expressions
{
    /// <summary>
    ///     Операнд-константа (числа типа int)
    /// </summary>
    public class IntConst : Expr
    {
        public int Num { get; set; }

		public IntConst(int num)
        {
            Num = num;
        }

        public override string ToString()
        {
            return Num.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Num == ((IntConst) obj).Num;
        }

        public override int GetHashCode()
        {
            return Num;
        }
		public static IntConst operator +(IntConst left, IntConst right)
		{
			return new IntConst(left.Num + right.Num);
		}
		public static IntConst operator -(IntConst left, IntConst right)
		{
			return new IntConst(left.Num - right.Num);
		}
		public static IntConst operator *(IntConst left, IntConst right)
		{
			return new IntConst(left.Num * right.Num);
		}
		public static IntConst operator /(IntConst left, IntConst right)
		{
			if (right.Num == 0)
				throw new DivideByZeroException();
			return new IntConst(left.Num / right.Num);
		}
		public static bool operator ==(IntConst left, IntConst right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntConst left, IntConst right)
        {
            return !Equals(left, right);
        }
    }
}