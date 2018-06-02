using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Оператор печати
    /// </summary>
    public class Print : Node
    {
        public Expr Data { get; set; }

        public string Sep { get; set; }

        public override string ToString()
        {
            return $"{TACodeNameManager.Instance[Label]} : print {Data} \"{Sep}\"";
        }
    }
}