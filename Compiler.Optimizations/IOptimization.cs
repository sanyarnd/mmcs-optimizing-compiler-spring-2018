using System.Collections.Generic;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Optimizations
{
    /// <summary>
    ///     Интерфейс для оптимизаторов кода
    /// </summary>
    public interface IOptimization
    {
        /// <summary>
        ///     Выполнить оптимизацию
        /// </summary>
        /// <param name="nodes">входные строки кода</param>
        /// <param name="applied">была ли применена оптимизация</param>
        /// <returns>результат применения оптимизации</returns>
        List<Node> Optimize(List<Node> nodes, out bool applied);
    }
}
