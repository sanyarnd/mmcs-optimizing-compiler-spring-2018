namespace Compiler.ThreeAddrCode.Expressions
{
    /// <summary>
    ///     Операнд-константа (числа типа int)
    /// </summary>
    public class IntConst : Expr
    {
        public IntConst(int num)
        {
            Num = num;
        }

        public int Num { get; set; }

        public override string ToString()
        {
            return Num.ToString();
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case IntConst ic:
                    return Num == ic.Num;
                case int i:
                    return Num == i;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Num.GetHashCode();
        }
    }
}