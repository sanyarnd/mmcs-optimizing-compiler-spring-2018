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
    using ActiveVar = HashSet<DUVarBase>;
    // Def множество 
    using DSet = HashSet<DUVarBase>;
    // Use множество
    using USet = HashSet<DUVarBase>;

    /// <summary>
    /// Класс итеративного алгоритма для 
    /// активных переменных
    /// </summary>
    class IterativeAlgorithmAV
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
        private Dictionary<int, ActiveVar> INb;
        /// <summary>
        /// Список активных переменных на выходе
        /// для каждого блока в CFG
        /// </summary>
        private Dictionary<int, ActiveVar> OUTb;
        /// <summary>
        /// Def множество для каждого блока
        /// в CFG
        /// </summary>
        private Dictionary<int, DSet> DefSet;
        /// <summary>
        /// Use множество для каждого блока
        /// в CFG
        /// </summary>
        private Dictionary<int, USet> UseSet;

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
            INb = new Dictionary<int, ActiveVar>();
            OUTb = new Dictionary<int, ActiveVar>();
            DefSet = new Dictionary<int, ActiveVar>();
            UseSet = new Dictionary<int, ActiveVar>();

            var it = CFG.CFGNodes.GetEnumerator();

            do
            {
                var B = it.Current.Block;

                INb.Add(B.BlockId, new ActiveVar());
                OUTb.Add(B.BlockId, new ActiveVar());

                var duSets = new DUSets(B);

                DefSet.Add(B.BlockId, duSets.DSet);
                UseSet.Add(B.BlockId, duSets.USet);
            }
            while (it.MoveNext());
        }

		private bool EqualIN(Dictionary<int, ActiveVar> obj1, Dictionary<int, ActiveVar> obj2)
		{
			return EqualINHelp(obj1, obj2) && EqualINHelp(obj2, obj1);
		}
		private bool EqualINHelp(Dictionary<int, ActiveVar> obj1, Dictionary<int, ActiveVar> obj2)
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
			var oldSetIN = new Dictionary<int, ActiveVar>();

			while (!EqualIN(oldSetIN, INb))
            {
				oldSetIN = new Dictionary<int, ActiveVar>(INb);
				var it = CFG.CFGNodes.GetEnumerator();

                do
                {
                    var B = it.Current;

                    // выходной блок
                    if (B.Children.Count == 0)
                        continue;

					var idB = B.Block.BlockId;

					// Первое уравнение
					foreach (var child in B.Children)
					{
						var idCh = child.Block.BlockId;
						OUTb[idB].Union(INb[idCh]);
					}

					var subUnion = new ActiveVar(OUTb[idB]);
					subUnion.ExceptWith(DefSet[idB]);

					// Второе уравнение
					INb[idB].Union(UseSet[idB]).Union(subUnion);
				}
                while (it.MoveNext());
            }

			var _it = CFG.CFGNodes.GetEnumerator();

			do
			{
				var B = _it.Current;
				var idB = B.Block.BlockId;

				if (B.Children.Count == 0)
				{
					IN.Union(INb[idB]);
					OUT.Union(OUTb[idB]);
				}

			}
			while (_it.MoveNext());
		}
    }
}
