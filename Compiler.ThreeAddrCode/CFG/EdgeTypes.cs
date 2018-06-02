using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Compiler.ThreeAddrCode.CFG
{
    public class EdgeTypes : Dictionary<Edge<BasicBlock>, EdgeType>
    {
        public override string ToString()
        {
            return string.Join("\n", this.Select(ed => $"[{ed.Key.Source.ToString()} -> {ed.Key.Target.ToString()}]: {ed.Value}"));
        }
    }
}