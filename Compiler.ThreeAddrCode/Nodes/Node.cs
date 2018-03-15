using Compiler.ThreeAddrCode.CFG;
using System;

namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Базовый узел
    /// </summary>
    public class Node
    {
        public Node()
        {
            Label = Guid.NewGuid();
        }

        /// <summary>
        ///     Уникальная метка-идентификатор
        /// </summary>
        public Guid Label { get; set; }

        /// <summary>
        ///     К какому базовому блоку принадлежит строка
        /// </summary>
        public BasicBlock Block { get; set; }

        /// <summary>
        ///     Флаг наличия перехода по goto на эту строку кода
        /// </summary>
        public bool IsLabeled { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Node node)
                return Label == node.Label;
            return false;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }
    }
}