using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ThreeAddrCode
{
    /// <summary>
    ///     Класс для хранения программы в формате трехадресного кода
    ///     Трехадресный код имеет следующий вид:
    ///     <para>Result := Left Operation Right; Result, Left, Right - операнды, Operation - бинарная операция</para>
    ///     <para>Код программы хранится в виде команд в формате трехадресного кода в списке</para>
    /// </summary>
    public class TACode
    {
        /// <summary>
        ///     Список команд программы в трехадресном формате
        /// </summary>
        public List<Node> CodeList { get; set; }

        /// <summary>
        ///     Словарь соответствий меток и узлов
        /// </summary>
        public Dictionary<Guid, Node> LabeledCode { get; }

        /// <summary>
        ///     Конструктор программы в формате трехадресного кода
        /// </summary>
        public TACode()
        {
            CodeList = new List<Node>();
            LabeledCode = new Dictionary<Guid, Node>();
        }

        /// <summary>
        ///     Добавить оператор
        /// </summary>
        public void AddNode(Node node)
        {
            CodeList.Add(node);
            LabeledCode.Add(node.Label, node);
        }

        /// <summary>
        ///     Удалить оператор
        /// </summary>
        /// <param name="node">Оператор</param>
        public bool RemoveNode(Node node)
        {
            return CodeList.Remove(node);
        }

        /// <summary>
        ///     Удалить набор операторов
        /// </summary>
        /// <param name="nodes">Список</param>
        public void RemoveNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
                CodeList.Remove(node);
        }

        /// <summary>
        ///     Построить список базовых блоков по программе в формате трехадресного кода
        /// </summary>
        /// <returns>список базовых блоков</returns>
        public IEnumerable<BasicBlock> CreateBasicBlockList()
        {
            var basicBlockList = new List<BasicBlock>();

            // список лидеров -- хранит "номера строк", 0 -- всегда лидер
            var leaders = new List<int> { 0 };

            var commands = CodeList.ToList();
            for (var i = 1; i < commands.Count; ++i)
            {
                var node = commands[i];

                // если в узел есть переход по GoTo
                if (node.IsLabeled && !leaders.Contains(i))
                    leaders.Add(i);
                // если узел является переходом GoTo
                if (node is Goto)
                    leaders.Add(i + 1);
            }

            // добавляем финальную команду в список для правильной группировки пар
            leaders.Add(commands.Count);

            // группируем список как набор пар:
            // [a0, a1, a2, a3, ...] -> [(a0, a1), (a1, a2), (a2, a3), ...]
            var ranges = leaders.Zip(leaders.Skip(1), Tuple.Create);
            foreach (var range in ranges)
            {
                var bbList = new List<Node>();

                for (var j = range.Item1; j < range.Item2; ++j)
                    bbList.Add(commands[j]);

                var bb = new BasicBlock(bbList);
                basicBlockList.Add(bb);
            }

            return basicBlockList;
        }

        public override string ToString()
        {
            return CodeList.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
        }
    }
}