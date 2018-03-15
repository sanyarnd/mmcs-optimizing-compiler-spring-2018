using Compiler.ThreeAddrCode.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ThreeAddrCode.CFG
{
    public class ControlFlowGraph
    {
        /// <summary>
        ///     Программа в формате трехадресного кода
        /// </summary>
        private readonly TACode _code;

        /// <summary>
        ///     Список базовых блоков программы (кэш)
        /// </summary>
        private readonly List<BasicBlock> _blockList;

        /// <summary>
        ///     Список узлов потока управления;
        ///     <para>Первый узел -- входной</para>
        /// </summary>
        public SortedSet<CFGNode> CFGNodes { get; }

        public ControlFlowGraph(TACode code)
        {
            _code = code;
            _blockList = _code.CreateBasicBlockList().ToList();

            CFGNodes = new SortedSet<CFGNode>();
            CreateCFGNodes();
        }

        /// <summary>
        ///     Создать узлы графа потока управления программы
        /// </summary>
        private void CreateCFGNodes()
        {
            var labelDict = _code.LabeledCode;

            foreach (var block in _blockList)
            {
                var last = block.CodeList.Last();

                var cfgNode = new CFGNode(block);
                CFGNodes.Add(cfgNode);

                // блок содержит GoTo в последней строке
                if (last is Goto gt)
                {
                    // ищем на какую строку идет переход
                    var targetFirst = labelDict[gt.TargetLabel];
                    // забираем информацию о том, какому блоку принадлежит эта строка
                    var targetNode = new CFGNode(targetFirst.Block);
                    // устанавливаем связи cfgNode <-> targetNode
                    cfgNode.AddChild(targetNode);

                    // добавляем его в набор узлов CFG
                    CFGNodes.Add(targetNode);
                }

                // случай, когда есть переход на первую строку блока:
                // var first = block.CodeList.First();
                // if (first.IsLabeled)
                // рассматривать не нужно, в силу того, что мы пробежимся по всем базовым блокам
                // и тем самым в любом случае рано или поздно найдем нужную связь
            }

            // каждый блок является родителем последующего
            var nodeList = CFGNodes.ToList();
            for (int i = 0; i < nodeList.Count - 1; ++i)
            {
                var cur = nodeList[i];
                var next = nodeList[i + 1];

                cur.Children.Add(next);
                next.Parents.Add(cur);
            }
        }
    }
}