using Compiler.ThreeAddrCode.Nodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuickGraph;

namespace Compiler.ThreeAddrCode.CFG
{
    public class ControlFlowGraph
    {

        /// <summary>
        ///     Список узлов потока управления;
        ///     <para>Первый узел -- входной</para>
        /// </summary>
        public ReadOnlyCollection<BasicBlock> CFGNodes => _cfgNodes.AsReadOnly();

        public TACode Code { get; }

        private readonly List<BasicBlock> _cfgNodes;

        public BidirectionalGraph<BasicBlock, Edge<BasicBlock>> CFGAuxiliary =
            new BidirectionalGraph<BasicBlock, Edge<BasicBlock>>();

        public EdgeTypes EdgeTypes { get; set; }


        public void ReverseCFGNodes()
        {
            _cfgNodes.Reverse();
        }
        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="code">экземпляр программы в формате трехадресного кода</param>
        public ControlFlowGraph(TACode code)
        {
            Code = code;
            _cfgNodes = new List<BasicBlock>();

            CreateCFGNodes();
        }

        /// <summary>
        ///     Создать узлы графа потока управления программы
        /// </summary>
        private void CreateCFGNodes()
        {
            // оборачиваем ББ в CFG
            foreach (var block in Code.CreateBasicBlockList())
            {
                _cfgNodes.Add(block);
                CFGAuxiliary.AddVertex(block);
            }

            foreach (var cfgNode in _cfgNodes)
            {
                // блок содержит GoTo в последней строке
                if (cfgNode.CodeList.Last() is Goto gt)
                {
                    // ищем на какую строку идет переход
                    var targetFirst = Code.LabeledCode[gt.TargetLabel];

                    // забираем информацию о том, какому блоку принадлежит эта строка
                    var targetNode = _cfgNodes.First(n => n.Equals(targetFirst.Block));

                    // устанавливаем связи cfgNode <-> targetNode
                    cfgNode.AddChild(targetNode);
                    targetNode.AddParent(cfgNode);

                    CFGAuxiliary.AddEdge(new Edge<BasicBlock>(cfgNode, targetNode));
                }
            }

            // каждый блок является родителем последующего
            var nodeList = CFGNodes.ToList();
            for (int i = 0; i < nodeList.Count - 1; ++i)
            {
                var cur = nodeList[i];
                var next = nodeList[i + 1];

                // если последняя строчка -- чистый goto (не if), то дуги быть не может
                if (cur.CodeList.Last().GetType() == typeof(Goto)) continue;

                cur.AddChild(next);
                next.AddParent(cur);

                CFGAuxiliary.AddEdge(new Edge<BasicBlock>(cur, next));
            }

            EdgeTypes = new EdgeTypes();
            //ClassificateEdges();
        }

        public void ClassificateEdges()
        {
            var depthTree = new DepthSpanningTree(this);
            foreach (var edge in CFGAuxiliary.Edges)
            {
                if (depthTree.SpanningTree.Edges.Any(e => e.Target.Equals(edge.Target) && e.Source.Equals(edge.Source)))
                {
                    EdgeTypes.Add(edge, EdgeType.Coming);
                }
                else if (depthTree.FindBackwardPath(edge.Source, edge.Target))
                {
                    EdgeTypes.Add(edge, EdgeType.Retreating);
                }
                else
                {
                    EdgeTypes.Add(edge, EdgeType.Cross);
                }
            }
        }

        public int NumberOfVertices()
        {
            return CFGNodes.Count;
        }

        public BasicBlock GetRoot()
        {
            return (NumberOfVertices() > 0) ? CFGNodes.ElementAt(0) : null;
        }

        /// <summary>
        /// Возвращает true, если CFG приводим, иначе - false
        /// </summary>
        public bool IsReducible { get => isReducible(); }

        /// <summary>
        /// Проверка CFG на приводимость
        /// </summary>
        /// <returns>Возвращает true, если CFG приводим, иначе - false</returns>
        private bool isReducible()
        {
            //Если ребра не классифицированы
            if (EdgeTypes.Count == 0)
                this.ClassificateEdges();
            //Отбираем все обратные дуги
            var retreatiangEdges = EdgeTypes.Where(elem => elem.Value == EdgeType.Retreating).Select(pair => pair.Key);
            var count = retreatiangEdges.Count();
            //Если таковых нет - CFG приводим
            if (retreatiangEdges.Count() == 0)
                return true;
            //Строим дерево доминанто
            var domMatrix = new DominatorTree(this).Matrix;
            //Проверяем каждую обратную дугу на обратимость(target доминирует на source в DominatorTree)
            foreach (var edge in retreatiangEdges)
            {
                //DominatorTree.matrix(i, j).HasLine <=> j dom i. (c) Max
                var rowWithSource = domMatrix.First(row => row.BasicBlock.BlockId == edge.Source.BlockId);
                var edgeInDomTree = rowWithSource.ItemDoms.First(cell => cell.BasicBlock.BlockId == edge.Target.BlockId);
                if (!edgeInDomTree.HasLine)
                    return false;
            }
            return true;
        }
    }
}