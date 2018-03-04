using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;

namespace Compiler.ThreeAddrCode.CFG
{
    /// <summary>
    ///     Базовый блок участка программы
    /// </summary>
    public class BasicBlock
    {
        /// <summary>
        ///     Идентификатор блока
        /// </summary>
        public int BlockId { get; }

        /// <summary>
        ///     Список узлов программы в трехадресной форме, связанных с блоком
        /// </summary>
        public IEnumerable<Node> CodeList => _codeList.AsReadOnly();

        private readonly List<Node> _codeList;

        /// <summary>
        ///     Конструктор базового блока
        /// </summary>
        /// <param name="codeList">список узлов программы в трехадресной форме</param>
        public BasicBlock(List<Node> codeList)
        {
            BlockId = codeList.GetHashCode();
            _codeList = codeList;

            foreach (var node in codeList)
                node.Block = this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return BlockId == ((BasicBlock) obj).BlockId;
        }

        public override int GetHashCode()
        {
            return BlockId;
        }

        public static bool operator ==(BasicBlock left, BasicBlock right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BasicBlock left, BasicBlock right)
        {
            return !Equals(left, right);
        }
    }
}