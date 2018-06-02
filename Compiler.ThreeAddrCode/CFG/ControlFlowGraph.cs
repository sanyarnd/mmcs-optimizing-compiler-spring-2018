﻿using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.DominatorTree;
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
            ClassificateEdges();
        }

        private void ClassificateEdges()
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
        /// Проверка CFG на приводимость
        /// </summary>
        /// <returns>Возвращает true, если CFG приводим, иначе - false</returns>
        public bool isReducible()
        {
            //Отбираем все обратные дуги
            var retreatiangEdges = EdgeTypes.Where(elem => elem.Value == EdgeType.Retreating).Select(pair => pair.Key);
            //Если таковых нет - CFG приводим
            if (retreatiangEdges.Count() == 0)
                return true;
            //Строим дерево доминанто
            var dominatorTree = new DominatorTree.DominatorTree();
            dominatorTree.CreateDomMatrix(this);
            //Проверяем каждую обратную дугу на обратимость
            foreach (var edge in retreatiangEdges)
            {
                dominatorTree.Matrix;
            }
            return true;
        }
    }
}