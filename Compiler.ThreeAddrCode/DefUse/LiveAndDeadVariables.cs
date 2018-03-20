using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode
{
    // Список мертвых переменных
    using DVars = List<DUVar>;
    // Список живых переменных
    using LVars = List<DUVar>;

    /// <summary>
    /// Класс для определения живых и мертвых переменных
    /// и удаления метвого кода
    /// </summary>
    class LiveAndDeadVariables
    {
        /// <summary>
        /// Базовый блок
        /// </summary>
		public BasicBlock Block { get; }
        /// <summary>
        /// Список мертвых переменных
        /// </summary>
        public DVars DeadVars { get; }
        /// <summary>
        /// Список живых переменных
        /// </summary>
        public LVars LiveVars { get; }

        /// <summary>
        /// Конструктор для класса определения
        /// живых и мертвых переменных для
        /// базового блока
        /// </summary>
        /// <param name="block">Базовый блок</param>
        public LiveAndDeadVariables(BasicBlock block)
        {
            this.Block = block;
            DeadVars = new DVars();
            LiveVars = new LVars();
            DefineLDVars(Block.CodeList, DeadVars, LiveVars);
        }

        /// <summary>
        /// Конструктор для класса определения
        /// живых и мертвых переменных для произвольного
        /// фрагмента кода
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
        public LiveAndDeadVariables(List<Node> nodes) : this(new BasicBlock(nodes, -1)) { }

        /// <summary>
        /// Определение живых и мертвых переменных
        /// в базовом блоке
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
        private void DefineLDVars(List<Node> nodes, DVars DeadVars, LVars LiveVars)
        {
            // Построение Def цепочки по блоку
            var dList = (new DULists(nodes)).DList;

            // Обход Def цепочки
            foreach (var dVar in dList)
                // Мертвая переменная (нигде не используется)
                if (dVar.UseVariables.Count == 0)
                    DeadVars.Add(dVar.DefVariable.Clone());
                // Живые переменные
                else
                {
                    LiveVars.Add(dVar.DefVariable.Clone());

                    foreach (var uVar in dVar.UseVariables)
                        LiveVars.Add(uVar.Clone());
                }                
        }

        /// <summary>
        /// Удаление мертвого кода в блоке
        /// </summary>
        public BasicBlock RemoveDeadCode()
        {
            var listNode = Block.CodeList;
            var deadVars = DeadVars;
            var liveVars = LiveVars;

            // Пока мы не удалим все мертвые переменные
            while (deadVars.Count != 0)
            {
                foreach (var dV in deadVars)
                    listNode.RemoveAll(x => x.Label == dV.StringId);

                deadVars.Clear();
                liveVars.Clear();

                // Определение живости/мертвости переменных
                DefineLDVars(listNode, deadVars, liveVars);
            }           

            return new BasicBlock(listNode, Block.BlockId);
        }

        public override string ToString()
        {
            return Block.ToString();
        }

        public override bool Equals(object obj)
        {
            return Block.Equals((obj as LiveAndDeadVariables).Block) &&
                DeadVars.Equals((obj as LiveAndDeadVariables).DeadVars) &&
                LiveVars.Equals((obj as LiveAndDeadVariables).LiveVars);
        }

        public override int GetHashCode()
        {
            return Block.GetHashCode() + DeadVars.GetHashCode() +
                LiveVars.GetHashCode();
        }
    }
}
