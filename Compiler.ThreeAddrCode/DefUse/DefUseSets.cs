using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;

namespace Compiler.ThreeAddrCode
{
    // Def множество 
    using DSet = HashSet<DUVarBase>;
    // Use множество
    using USet = HashSet<DUVarBase>;

    /// <summary>
    /// Класс для построения Def и Use множеств
    /// в пределах одного блока
    /// </summary>
    class DUSets
    {
        /// <summary>
        /// Базовый блок
        /// </summary>
		public BasicBlock Block { get; }
        /// <summary>
        /// Def множество
        /// </summary>
        public DSet DSet { get; }
        /// <summary>
        /// Use множество
        /// </summary>
        public USet USet { get; }

        /// <summary>
        /// Конструктор для класса, генерирующий
        /// Def и Use множества по базовому блоку
        /// </summary>
        /// <param name="block">Базовый блок</param>
		public DUSets(BasicBlock block)
        {
            this.Block = block;
            DSet = new DSet();
            USet = new USet();
            BuildDUSets();
        }

        /// <summary>
        /// Конструктор для класса, генерирующий
        /// Def и Use множества по произвольному
        /// фрагменту кода
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
		public DUSets(List<Node> nodes) : this(new BasicBlock(nodes, -1)) { }

        /// <summary>
        /// Создает Def и Use множества для базового блока
        /// </summary>
        private void BuildDUSets()
        {
            var duLists = new DULists(Block);

            foreach (var d in duLists.DList)
                DSet.Add(d.DefVariable);

            foreach (var u in duLists.UListNotValid)
                USet.Add(u);
        }
    }
}
