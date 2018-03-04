using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ThreeAddrCode.CFG
{
    /// <summary>
    /// Класс-обёртка, который содержит в себе экземпляр базового блока и списки его детей и потомков
    /// </summary>
    public class CFGNode
    {
        /// <summary>
        ///     Базовый блок
        /// </summary>
        public BasicBlock Block { get; }

        /// <summary>
        ///     Дети узла CFG графа
        /// </summary>
        public IEnumerable<CFGNode> Children => _children.ToList();

        private readonly List<CFGNode> _children;

        /// <summary>
        ///     Родители узла CFG графа
        /// </summary>
        public IEnumerable<CFGNode> Parents => _parents.ToList();

        private readonly List<CFGNode> _parents;

        /// <summary>
        ///     Конструктор узла графа CFG
        /// </summary>
        /// <param name="block">базовый блок</param>
        public CFGNode(BasicBlock block)
        {
            Block = block ?? throw new NullReferenceException("Block cannot be null");
            _children = new List<CFGNode>();
            _parents = new List<CFGNode>();
        }

        /// <summary>
        ///     Связать узел с другим по принципу родитель -> ребенок
        /// </summary>
        /// <param name="child">дочерний узел</param>
        public void AddChild(CFGNode child)
        {
            if (!_children.Contains(child))
                _children.Add(child);
        }

        /// <summary>
        ///     Связать узел с другим по принципу ребенок -> родитель
        /// </summary>
        /// <param name="parent">родитель</param>
        public void AddParent(CFGNode parent)
        {
            if (!_parents.Contains(parent))
                _parents.Add(parent);
        }
    }
}