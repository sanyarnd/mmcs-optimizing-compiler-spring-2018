namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Пустой оператор
    /// </summary>
    public class Empty : Node
    {
        public override string ToString()
        {
            return $"{Label} : nop";
        }
    }
}