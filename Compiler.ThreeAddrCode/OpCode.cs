namespace Compiler.ThreeAddrCode
{
    /// <summary>
    ///     Перечисление возможных операций в трехадресном коде
    /// </summary>
    public enum OpCode
    {
        Plus,
        Minus,
        Mul,
        Div,

        UnaryMinus,
        Copy,

        Greater,
        Less,
        GreaterEq,
        LessEq,
        Equal,
        NotEqual,
        Not
    }
}