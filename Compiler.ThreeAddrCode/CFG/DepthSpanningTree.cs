using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Graphviz;

namespace Compiler.ThreeAddrCode.CFG
{
    public class DepthSpanningTree
    {
        public HashSet<BasicBlock> Visited { get; }
        public Dictionary<BasicBlock, int> Numbers { get; }
        public BidirectionalGraph<BasicBlock, Edge<BasicBlock>> SpanningTree { get; }

        public DepthSpanningTree(ControlFlowGraph controlFlowGraph)
        {
            int numberOfVertices = controlFlowGraph.NumberOfVertices() - 1;
            Visited = new HashSet<BasicBlock>();
            SpanningTree = new BidirectionalGraph<BasicBlock, Edge<BasicBlock>>();
            Numbers = new Dictionary<BasicBlock, int>();

            var rootBlock = controlFlowGraph.GetRoot();
            BuildTree(rootBlock, ref numberOfVertices);
        }

        private void BuildTree(BasicBlock block, ref int currentNumber)
        {
            if (block == null)
                return;

            Visited.Add(block);

            if (block.Children == null)
            {
                Numbers[block] = currentNumber;
                return;
            }

            var children = new List<BasicBlock>();
            if (block.Children != null)
                children.AddRange(block.Children);

            foreach (var child in children)
            {
                if (!Visited.Contains(child))
                {
                    if (!SpanningTree.Vertices.Contains(block))
                        SpanningTree.AddVertex(block);

                    if (!SpanningTree.Vertices.Contains(child))
                        SpanningTree.AddVertex(child);

                    SpanningTree.AddEdge(new Edge<BasicBlock>(block, child));
                    BuildTree(child, ref currentNumber);
                }

                Numbers[block] = currentNumber;
                currentNumber -= 1;
            }
        }

        public bool FindBackwardPath(BasicBlock source, BasicBlock target)
        {
            var result = false;

            var incomingEdges = SpanningTree.InEdges(source);
            while (incomingEdges.Count() > 0)
            {
                var edge = incomingEdges.First();
                if (edge.Source.Equals(target))
                {
                    result = true;
                    break;
                }
                else
                {
                    incomingEdges = SpanningTree.InEdges(edge.Source);
                }
            }

            return result;
        }

        public override string ToString()
        {
            var graphviz = new GraphvizAlgorithm<BasicBlock, Edge<BasicBlock>>(SpanningTree);
            return graphviz.Generate();
        }
    }
}