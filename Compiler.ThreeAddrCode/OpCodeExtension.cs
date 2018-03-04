namespace Compiler.ThreeAddrCode
{
    /// <summary>
    ///     Расширение для типов операций трехадресного кода
    /// </summary>
    public static class OpCodeExtension
    {
        public static string GetSymbol(this OpCode op)
        {
            switch (op)
            {
                case OpCode.Plus: return "+";
                case OpCode.UnaryMinus:
                case OpCode.Minus: return "-";
                case OpCode.Mul: return "*";
                case OpCode.Div: return "/";
                case OpCode.Copy: return "";
                case OpCode.Greater: return ">";
                case OpCode.Less: return "<";
                case OpCode.GreaterEq: return ">=";
                case OpCode.LessEq: return "<=";
                case OpCode.Equal: return "==";
                case OpCode.NotEqual: return "!=";
                case OpCode.Not: return "!";

                default: return "unknown";
            }
        }
    }
}