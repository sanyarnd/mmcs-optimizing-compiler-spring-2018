using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Оператор условного перехода
    /// </summary>
    public class IfGoto : Goto
    {
        /// <summary>
        ///     Условие перехода
        /// </summary>
        public Expr Condition { get; set; }

        public override string ToString()
        {
            return $"{TACodeNameManager.Instance[Label]} : if {Condition} goto {TACodeNameManager.Instance[TargetLabel]}";
        }
    }
}