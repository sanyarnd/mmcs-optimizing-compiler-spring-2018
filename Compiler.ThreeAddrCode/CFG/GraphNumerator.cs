using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ThreeAddrCode.CFG
{
    public static class GraphNumExt
    {
        public static IGraphNumerator Reverse(this GraphNumerator n, ControlFlowGraph g)
            => new ReverseNum(g, n);

        public static NumeratedGraph BackOrder(this TACode code)
        {
            var graph = new NumeratedGraph(code, null);
            graph.Numerator = GraphNumerator.BackOrder(graph);
            return graph;
        }

        public static NumeratedGraph StraightOrder(this TACode code)
        {
            var graph = new NumeratedGraph(code, null);
            graph.Numerator = GraphNumerator.BackOrder(graph).Reverse(graph);
            return graph;
        }

        private class ReverseNum : IGraphNumerator
        {
            private readonly ControlFlowGraph _graph;
            private readonly IGraphNumerator _num;

            public ReverseNum(ControlFlowGraph graph, IGraphNumerator num)
            {
                _graph = graph;
                _num = num;
            }

            public int? GetIndex(BasicBlock b)
            {
                var ind = _num.GetIndex(b);
                if (ind == null) return null;

                return _graph.CFGNodes.Count() - ind.Value;
            }
        }

    }

    public class GraphNumerator : IGraphNumerator
    {
        public static GraphNumerator BackOrder(ControlFlowGraph graph)
        {
            var root = graph.CFGNodes.ElementAt(0);
            var num = new GraphNumerator();
            var index = 0;
            var openSet = new HashSet<BasicBlock>();

            void Iter(BasicBlock node)
            {
                openSet.Add(node);
                var children = node.Children;
                foreach(var c in children.Where(x => !openSet.Contains(x)))
                    Iter(c);
                num._num[node] = index++;
            }

            Iter(root);
            return num;
        }

        private readonly Dictionary<BasicBlock, int> _num = new Dictionary<BasicBlock, int>();

        public virtual int? GetIndex(BasicBlock b)
            => _num.TryGetValue(b, out var res) ?
                new int?(res) : null;
    }

    public class NumeratedGraph : ControlFlowGraph
    {
        public NumeratedGraph(TACode code, IGraphNumerator numerator) : base(code)
        {
            Numerator = numerator;
        }

        public IGraphNumerator Numerator { get; set;  }

        public int? IndexOf(BasicBlock b) => Numerator.GetIndex(b);

        private string NodeToString(BasicBlock n)
        {
            var blockName = TACodeNameManager.Instance[n.BlockId];
            var index = Numerator?.GetIndex(n);

            return $"({index}:{blockName})";
        }

        public override string ToString()
        {
            var s = new StringBuilder();

            foreach (var n in CFGNodes)
                s.AppendLine(
                        $"{NodeToString(n)} : [{ String.Join(", ", n.Children.Select(NodeToString)) }]"
                    );
            return s.ToString();
        }
    }


    public interface IGraphNumerator
    {
        int? GetIndex(BasicBlock b);
    }
}
