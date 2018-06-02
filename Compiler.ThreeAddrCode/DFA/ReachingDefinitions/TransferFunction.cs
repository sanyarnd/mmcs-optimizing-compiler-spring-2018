using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.ThreeAddrCode.DFA.ReachingDefinitions
{
    public class TransferFunction : ITransferFunction<HashSet<Guid>>
    {
        private TACode taCode;

        public TransferFunction(TACode ta) => taCode = ta;

        public HashSet<Guid> Transfer(BasicBlock basicBlock, HashSet<Guid> input, ILatticeOperations<HashSet<Guid>> ops)
        {
            var (gen, kill) = GetGenAndKill(basicBlock, ops);
            var inset = new HashSet<Guid>(input);
            return new HashSet<Guid>(inset.Except(kill).Union(gen));
        }

        public (HashSet<Guid>, HashSet<Guid>) GetGenAndKill (BasicBlock basicBlock, ILatticeOperations<HashSet<Guid>> ops)
        {
            var gen = new HashSet<Guid>(basicBlock.CodeList.Where(x => x is Assign).Select(x => x.Label));
            var vars = basicBlock.CodeList
                .Where(x => x is Assign)
                .Select(x => ((x as Assign).Result as Var).Id)
                .ToList();
            var ad = taCode.CodeList
                .Where(x => !gen.Contains(x.Label) && x is Assign)
                .Cast<Assign>()
                .Where(x => vars.Contains((x.Result as Var).Id))
                .Select(x => x.Label);
            var kill = new HashSet<Guid>(ad);
            return (gen, kill);
        }

        
    }
}
