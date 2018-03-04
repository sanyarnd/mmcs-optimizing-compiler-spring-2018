using Compiler.ThreeAddrCode.CFG;
using System;

namespace Compiler.ThreeAddrCode.Nodes
{
    /// <summary>
    ///     Базовый узел
    /// </summary>
    public class Node
    {
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
        
        public Node()
        {
            Label = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Label.Equals(((Node) obj).Label);
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }
    }
}