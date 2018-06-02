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
    using ActiveVar = HashSet<Var>;
    // Def множество 
    using DSet = HashSet<Var>;
    // Use множество
    using USet = HashSet<Var>;

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
        /// Список активных переменных на входе
        /// для каждого блока в CFG
        /// </summary>
        public Dictionary<Guid, ActiveVar> IN;
        /// <summary>
        /// Список активных переменных на выходе
        /// для каждого блока в CFG
        /// </summary>
        public Dictionary<Guid, ActiveVar> OUT;
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
            StartSettings();
            Algorithm();
        }

        /// <summary>
        /// Стартовые настройки алгоритма
        /// </summary>
        private void StartSettings()
        {
            IN = new Dictionary<Guid, ActiveVar>();
            OUT = new Dictionary<Guid, ActiveVar>();
            DefSet = new Dictionary<Guid, DSet>();
            UseSet = new Dictionary<Guid, USet>();

            foreach (var B in CFG.CFGNodes)
            {
                IN.Add(B.BlockId, new ActiveVar());
                OUT.Add(B.BlockId, new ActiveVar());

                var duSets = new DUSets(B);

                DefSet.Add(B.BlockId, duSets.DSet);
                UseSet.Add(B.BlockId, duSets.USet);
            }
        }

        /// <summary>
        /// Сравнение двух словарей
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
		private bool EqualIN(Dictionary<Guid, ActiveVar> obj1, Dictionary<Guid, ActiveVar> obj2)
		{
            var IsEqual = true;

            foreach (var v in obj1)
            {
                if (v.Value.Count != obj2[v.Key].Count)
                    return false;

                for (var i = 0; i < v.Value.Count; i++)
                    IsEqual &= v.Value.ElementAt(i) == obj2[v.Key].ElementAt(i);
            }         

            return IsEqual;
		}

        /// <summary>
        /// Копирование словаря
        /// </summary>
        /// <returns></returns>
        private Dictionary<Guid, ActiveVar> CopyIN()
        {
            var oldSetIN = new Dictionary<Guid, ActiveVar>();

            foreach (var elem in IN)
            {
                ActiveVar AV = new ActiveVar();

                foreach (var v in elem.Value)
                    AV.Add(v);

                oldSetIN.Add(elem.Key, AV);
            }

            return oldSetIN;
        }

        /// <summary>
        /// Базовый итеративный алгоритм
        /// </summary>
        private void Algorithm()
        {
            Dictionary<Guid, ActiveVar> oldSetIN;

            do
            {
                oldSetIN = CopyIN();

                foreach (var B in CFG.CFGNodes)
                {
                    var idB = B.BlockId;

                    // Первое уравнение
                    foreach (var child in B.Children)
                    {
                        var idCh = child.BlockId;
                        OUT[idB].UnionWith(IN[idCh]);
                    }

                    var subUnion = new ActiveVar(OUT[idB]);
                    subUnion.ExceptWith(DefSet[idB]);

                    // Второе уравнение
                    IN[idB].UnionWith(UseSet[idB]);
                    IN[idB].UnionWith(subUnion);
                }
            }
            while (!EqualIN(oldSetIN, IN));
		}
    }
}
