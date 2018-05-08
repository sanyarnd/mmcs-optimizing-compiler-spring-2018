using System;
using System.Collections.Generic;

namespace Compiler.ThreeAddrCode.DFA.ReachingExpressions
{
    /// <summary>
    ///     Анализатор доступных выражений в графе потока управления.
    /// </summary>
    /// <returns>Множества выражений, доступных на входе и выходе из каждого блока графа потока.</returns>
    public class ReachingExpressions 
    {
        public ISet<Guid> In { get; }
        public ISet<Guid> Out { get; }

        public ReachingExpressions()
        {
            throw new NotImplementedException();
        }
    }
}