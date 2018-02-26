using System.Collections.Generic;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Optimizations
{
    public interface IOptimization
    {
        List<Node> Optimize(List<Node> nodes);
    }
}
