using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Оператор присваивания
    /// </summary>
    public class Assign : Node
    {
        /// <summary>
        ///     Левый операнд,
        ///     null, если операция унарная
        /// </summary>
        public Expr Left { get; set; }

        /// <summary>
        ///     Правый операнд
        /// </summary>
        public Expr Right { get; set; }

        /// <summary>
        ///     Хранилище результата
        /// </summary>
        public Var Result { get; set; }

        /// <summary>
        ///     Производимая операция
        /// </summary>
        public OpCode Operation { get; set; }

        public override string ToString()
        {
            if (Left == null)
                return $"{TACodeNameManager.Instance[Label]} : {Result} = {Operation.GetSymbol()}{Right}";
            return $"{TACodeNameManager.Instance[Label]} : {Result} = {Left} {Operation.GetSymbol()} {Right}";
        }
    }
}