using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ThreeAddrCode.DFA.ReachingDefinitions
{
    public class Operations : ILatticeOperations<HashSet<Guid>>
    {
        private HashSet<Guid> upper;

        public Operations(TACode code)
        {
            upper = new HashSet<Guid>(code.CodeList.Select(x => (Guid)x.Label));
        }

        public HashSet<Guid> Lower => new HashSet<Guid>();

        public HashSet<Guid> Upper => upper;

        public bool? Compare(HashSet<Guid> a, HashSet<Guid> b)
        {
            var symmDiff = a.Except(b).Union(b.Except(a));
            if (symmDiff.Count() == 0)
                return null;
            return b.IsSubsetOf(a);
        }

        public HashSet<Guid> Operator(HashSet<Guid> a, HashSet<Guid> b) => 
            new HashSet<Guid>(a.Union(b));
    }
}
