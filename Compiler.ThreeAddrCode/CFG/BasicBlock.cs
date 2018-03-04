using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;

namespace Compiler.ThreeAddrCode.CFG
{
    /// <summary>
    ///     Базовый блок участка программы
    /// </summary>
    public class BasicBlock : IComparable<BasicBlock>
    {
        /// <summary>
        ///     Идентификатор блока
        /// </summary>
        public int BlockId { get; }

        /// <summary>
        ///     Список узлов программы в трехадресной форме, связанных с блоком
        /// </summary>
        public List<Node> CodeList { get; }

        /// <summary>
        ///     Конструктор базового блока
        /// </summary>
        /// <param name="codeList">список узлов программы в трехадресной форме</param>
        /// <param name="num">номер блока</param>
        /// <remarks>
        ///     важно! при создании упорядоченных коллекций блоков, чтобы номера блоков
        ///     были различны, т.к. на номер блока опирается компаратор
        /// </remarks>
        public BasicBlock(List<Node> codeList, int num)
        {
            BlockId = num;
            CodeList = codeList;

            foreach (var node in codeList)
                node.Block = this;
        }

        public int CompareTo(BasicBlock other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return BlockId.CompareTo(other.BlockId);
        }
    }
}