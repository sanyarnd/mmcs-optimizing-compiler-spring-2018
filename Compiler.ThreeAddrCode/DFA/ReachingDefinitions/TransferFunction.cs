using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA.ReachingDefinitions
{
    public class TransferFunction : ITransferFunction<HashSet<Guid>>
    {
        public HashSet<Guid> Transfer(BasicBlock basicBlock, HashSet<Guid> input, ILatticeOperations<HashSet<Guid>> ops)
        {
            var gen = new HashSet<Guid>(basicBlock.CodeList.Select(x => (Guid)x.Label));
            var kill = new HashSet<Guid>(ops.Upper.Except(gen));
            var inset = new HashSet<Guid>(input);
            return new HashSet<Guid>(inset.Except(kill).Union(gen));
        }
    }
}
