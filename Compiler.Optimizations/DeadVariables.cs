using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.Optimizations
{
    public class DeadVariables : IOptimization
    {
        private LiveAndDeadVariables _opt;

        public List<Node> Optimize(List<Node> nodes, out bool applied)
        {
            _opt = new LiveAndDeadVariables(nodes.ToList());
            var res = _opt.RemoveDeadCode().CodeList.ToList();
            applied = nodes.Except(res).Any();
            return res;
        }
    }
}
