using Compiler.ThreeAddrCode.CFG;
using System.Collections.Generic;

namespace Compiler.ThreeAddrCode.DominatorTree
{
    /// <summary>
    ///     Ячейка в матрице доминаторов
    /// </summary>
    public class DomCell
    {
        public BasicBlock BasicBlock { get; set; }
        public bool HasLine { get; set; }
    }

    /// <summary>
    ///     Строка в матрице доминаторов
    /// </summary>
    public class DomRow
    {
        public BasicBlock BasicBlock { get; set; }
        public List<DomCell> ItemDoms { get; set; } = new List<DomCell>();
    }

}
