using Compiler.ThreeAddrCode.Nodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ///     Список узлов потока управления;
        ///     <para>Первый узел -- входной</para>
        /// </summary>
        public ReadOnlyCollection<BasicBlock> CFGNodes => _cfgNodes.AsReadOnly();
        private readonly List<BasicBlock> _cfgNodes;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="code">экземпляр программы в формате трехадресного кода</param>
        public ControlFlowGraph(TACode code)
        {
            _code = code;
            _cfgNodes = new List<BasicBlock>();

            CreateCFGNodes();
        }

        /// <summary>
        ///     Создать узлы графа потока управления программы
        /// </summary>
        private void CreateCFGNodes()
        {
            // оборачиваем ББ в CFG
            foreach (var block in _code.CreateBasicBlockList())
                _cfgNodes.Add(block);

            foreach (var cfgNode in _cfgNodes)
            {
                // блок содержит GoTo в последней строке
                if (cfgNode.CodeList.Last() is Goto gt)
                {
                    // ищем на какую строку идет переход
                    var targetFirst = _code.LabeledCode[gt.TargetLabel];

                    // забираем информацию о том, какому блоку принадлежит эта строка
                    var targetNode = _cfgNodes.First(n => n.Equals(targetFirst.Block));

                    // устанавливаем связи cfgNode <-> targetNode
                    cfgNode.AddChild(targetNode);
                    targetNode.AddParent(cfgNode);
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
            }
        }
    }
}