using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.ThreeAddrCode.DFA;

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
        public Guid BlockId { get; }

        /// <summary>
        ///     Потомки базового блока
        /// </summary>
        public IEnumerable<BasicBlock> Children => _children.ToList();

        private readonly List<BasicBlock> _children = new List<BasicBlock>();

        /// <summary>
        ///     Родители базового блока
        /// </summary>
        public IEnumerable<BasicBlock> Parents => _parents.ToList();

        private readonly List<BasicBlock> _parents = new List<BasicBlock>();

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
            BlockId = Guid.NewGuid();
            _codeList = codeList;

            foreach (var node in codeList)
                node.Block = this;
        }

        /// <summary>
        ///     Связать узел с другим по принципу родитель -> ребенок
        /// </summary>
        /// <param name="child">дочерний узел</param>
        public void AddChild(BasicBlock child)
        {
            if (!_children.Contains(child))
                _children.Add(child);
        }

        /// <summary>
        ///     Связать узел с другим по принципу ребенок -> родитель
        /// </summary>
        /// <param name="parent">родитель</param>
        public void AddParent(BasicBlock parent)
        {
            if (!_parents.Contains(parent))
                _parents.Add(parent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return BlockId.Equals(((BasicBlock) obj).BlockId);
        }

        public override int GetHashCode()
        {
            return BlockId.GetHashCode();
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