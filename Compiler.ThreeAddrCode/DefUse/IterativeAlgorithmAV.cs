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
    // Тип для множества активных переменных
    using ActiveVar = HashSet<DUVar>;
    // Def множество 
    using DSet = HashSet<DUVar>;
    // Use множество
    using USet = HashSet<DUVar>;

    /// <summary>
    /// Класс итеративного алгоритма для 
    /// активных переменных
    /// </summary>
    public class IterativeAlgorithmAV
    {
        /// <summary>
        /// Граф потока управления
        /// </summary>
        public ControlFlowGraph CFG { get; }
        /// <summary>
        /// Множкство переменных активных
        /// на входе
        /// </summary>
        public ActiveVar IN { get; }
        /// <summary>
        /// Множкство переменных активных
        /// на выходе
        /// </summary>
        public ActiveVar OUT { get; }

        /// <summary>
        /// Список активных переменных на входе
        /// для каждого блока в CFG
        /// </summary>
        private Dictionary<Guid, ActiveVar> INb;
        /// <summary>
        /// Список активных переменных на выходе
        /// для каждого блока в CFG
        /// </summary>
        private Dictionary<Guid, ActiveVar> OUTb;
        /// <summary>
        /// Def множество для каждого блока
        /// в CFG
        /// </summary>
        private Dictionary<Guid, DSet> DefSet;
        /// <summary>
        /// Use множество для каждого блока
        /// в CFG
        /// </summary>
        private Dictionary<Guid, USet> UseSet;

        /// <summary>
        /// Класс для итеративного алгоритма 
        /// активных переменных
        /// </summary>
        /// <param name="CFG"></param>
        public IterativeAlgorithmAV(ControlFlowGraph CFG)
        {
            this.CFG = CFG;
            this.IN = new ActiveVar();
            this.OUT = new ActiveVar();
            StartSettings();
            Algorithm();
        }

        /// <summary>
        /// Стартовые настройки алгоритма
        /// </summary>
        private void StartSettings()
        {
            INb = new Dictionary<Guid, ActiveVar>();
            OUTb = new Dictionary<Guid, ActiveVar>();
            DefSet = new Dictionary<Guid, ActiveVar>();
            UseSet = new Dictionary<Guid, ActiveVar>();

            foreach (var B in CFG.CFGNodes)
            {
                INb.Add(B.BlockId, new ActiveVar());
                OUTb.Add(B.BlockId, new ActiveVar());

                var duSets = new DUSets(B);

                DefSet.Add(B.BlockId, duSets.DSet);
                UseSet.Add(B.BlockId, duSets.USet);
            }
        }

		private bool EqualIN(Dictionary<Guid, ActiveVar> obj1, Dictionary<Guid, ActiveVar> obj2)
		{
			return EqualINHelp(obj1, obj2) && EqualINHelp(obj2, obj1);
		}
		private bool EqualINHelp(Dictionary<Guid, ActiveVar> obj1, Dictionary<Guid, ActiveVar> obj2)
		{
			var result = true;

			foreach (var o in obj1)
				result &= obj2.Contains(o);

			return result;
		}

		/// <summary>
		/// Базовый итеративный алгоритм
		/// </summary>
		private void Algorithm()
        {
			var oldSetIN = new Dictionary<Guid, ActiveVar>();

			while (!EqualIN(oldSetIN, INb))
            {
				oldSetIN = new Dictionary<Guid, ActiveVar>(INb);

                foreach (var B in CFG.CFGNodes)
                {
                    // выходной блок
                    if (B.Children.Count() == 0)
                        continue;

					var idB = B.BlockId;

					// Первое уравнение
					foreach (var child in B.Children)
					{
						var idCh = child.BlockId;
						OUTb[idB].Union(INb[idCh]);
					}

					var subUnion = new ActiveVar(OUTb[idB]);
					subUnion.ExceptWith(DefSet[idB]);

					// Второе уравнение
					INb[idB].Union(UseSet[idB]).Union(subUnion);
				}
            }

            foreach (var B in CFG.CFGNodes)
            {
				var idB = B.BlockId;

				if (B.Children.Count() == 0)
				{
					IN.Union(INb[idB]);
					OUT.Union(OUTb[idB]);
				}

			}
		}
    }
}
