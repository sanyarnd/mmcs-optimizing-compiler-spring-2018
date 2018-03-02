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
        public List<DUVar> DeadVars { get; }
        /// <summary>
        /// Список живых переменных
        /// </summary>
        public List<DUVar> LiveVars { get; }

        /// <summary>
        /// Конструктор для класса определения
        /// живых и мертвых переменных
        /// </summary>
        /// <param name="block">Базовый блок</param>
        public LiveAndDeadVariables(BasicBlock block)
        {
            this.Block = block;
            DeadVars = new List<DUVar>();
            LiveVars = new List<DUVar>();
            DefineLDVars();
        }

        /// <summary>
        /// Определение живых и мертвых переменных
        /// в базовом блоке
        /// </summary>
        private void DefineLDVars()
        {

        }

        /// <summary>
        /// Удаление мертвого кода в блоке
        /// </summary>
        public BasicBlock RemoveDeadCode()
        {
            var listNode = new List<Node>();

            foreach (var command in Block.CodeList)
            {
                var index = DeadVars.FindIndex(dV =>
                {
                    return dV.StringId == command.Label;
                });

                if (index == -1)
                    listNode.Add(command); // метод command.Clone ???
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
