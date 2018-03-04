using System;
using System.Collections.Generic;

namespace Compiler.ThreeAddrCode.CFG
{
    /// <summary>
    /// Класс-обёртка, который содержит в себе экземпляр базового блока и списки его детей и потомков
    /// </summary>
    public class CFGNode : IComparable<CFGNode>
    {
        /// <summary>
        ///     Базовый блок
        /// </summary>
        public BasicBlock Block { get; }

        /// <summary>
        ///     Дети узла CFG графа
        /// </summary>
        public SortedSet<CFGNode> Children { get; }

        /// <summary>
        ///     Родители узла CFG графа
        /// </summary>
        public SortedSet<CFGNode> Parents { get; }

        /// <summary>
        ///     Конструктор узла графа CFG
        /// </summary>
        /// <param name="block">базовый блок</param>
        public CFGNode(BasicBlock block)
        {
            Block = block ?? throw new NullReferenceException("Block cannot be null");
            Children = new SortedSet<CFGNode>();
            Parents = new SortedSet<CFGNode>();
        }

        /// <summary>
        ///     Связать узел с другим по принципу родитель -> ребенок
        /// </summary>
        /// <param name="child">дочерний узел</param>
        public void AddChild(CFGNode child)
        {
            Children.Add(child);
            child.Parents.Add(this);
        }

        /// <summary>
        ///     Связать узел с другим по принципу ребенок -> родитель
        /// </summary>
        /// <param name="parent">родитель</param>
        public void AddParent(CFGNode parent)
        {
            Parents.Add(parent);
            parent.Children.Add(this);
        }

        public int CompareTo(CFGNode other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Comparer<BasicBlock>.Default.Compare(Block, other.Block);
        }
    }
}