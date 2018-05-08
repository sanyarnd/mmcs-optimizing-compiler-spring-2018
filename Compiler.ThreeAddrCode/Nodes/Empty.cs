namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Пустой оператор
    /// </summary>
    public class Empty : Node
    {
        public Empty(string name = "") : base(name) { }

        public override string ToString()
        {
            return $"{Label} : nop";
        }
    }
}