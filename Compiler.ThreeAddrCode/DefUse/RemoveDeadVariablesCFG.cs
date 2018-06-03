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
    // Список мертвых переменных
    using DVars = List<DUVar>;
    // Список живых переменных
    using LVars = List<DUVar>;

    /// <summary>
    /// Класс для удаления мертвого кода в CFG 
    /// </summary>
    public class RemoveDeadVariablesCFG
    {
        /// <summary>
        /// Граф исходной программы
        /// </summary>
        public TACode CodeIN { get; }
        /// <summary>
        /// Граф программы без мертвого кода
        /// </summary>
        public TACode  CodeNew { get; }

        /// <summary>
        /// Список мертвых переменных
        /// </summary>
        private DVars deadVars;
        /// <summary>
        /// Список живых переменных
        /// </summary>
        private LVars liveVars;
        /// <summary>
        /// Список активных переменных на выходе
        /// для каждого блока в CFG
        /// </summary>
        private Dictionary<Guid, ActiveVar> OUT;

        /// <summary>
        /// Класс для удаления мертвого кода 
        /// в CFG
        /// </summary>
        /// <param name="CFG"></param>
        public RemoveDeadVariablesCFG(TACode code)
        {
            this.CodeIN = code;
            this.deadVars = new DVars();
            this.liveVars = new LVars();
            this.CodeNew = RemoveDeadCodeInCFG();
        }

        private NumeratedGraph CreateCFG(TACode code)
        {
            var cfg = new NumeratedGraph(code, null);
            cfg.Numerator = GraphNumerator.BackOrder(cfg).Reverse(cfg);
            return cfg;
        }

        /// <summary>
        /// Удаление мертвого кода в CFG
        /// </summary>
        /// <returns></returns>
        private TACode RemoveDeadCodeInCFG()
        {
            var code = new TACode();
            code.CodeList = CodeIN.CodeList.ToList();
            NumeratedGraph cfg;
            int countRemove;

            do
            {
                // Вычисляем CFG
                cfg = CreateCFG(code);
                // Вычисляем OUT переменные для всех блоков в CFG
                this.OUT = (new IterativeAlgorithmAV(cfg)).OUT;
                countRemove = 0;

                // Для каждого блока в cfg
                foreach (var B in cfg.CFGNodes)
                {
                    // Удаляем мертвые строки кода
                    var newB = RemoveDeadCodeInBlock(B);
                    var curCountRem = B.CodeList.Count() - newB.CodeList.Count();

                    if (curCountRem != 0)
                    {
                        var idxStart = CalculateIdxStart(cfg, cfg.IndexOf(B).Value) - countRemove;
                        var len = B.CodeList.Count();
                        code = ReplaceCode(code, newB.CodeList.ToList(), idxStart, len);
                        countRemove += curCountRem;
                    }
                }
            }
            while (countRemove != 0);

            return code;
        }

        /// <summary>
        /// Вычисляет индекс в исходном коде
        /// </summary>
        /// <returns></returns>
        private int CalculateIdxStart(NumeratedGraph cfg, int idxB)
        {
            var idxStart = 0;

            foreach (var B in cfg.CFGNodes)
                if (cfg.IndexOf(B).Value < idxB)
                    idxStart += B.CodeList.Count();

            return idxStart;
        }

        /// <summary>
        /// Заменияет код
        /// </summary>
        /// <returns></returns>
        private TACode ReplaceCode(TACode code, List<Node> nodes, int idxStart, int len)
        {
            var newCode = new List<Node>();

            newCode.AddRange(code.CodeList.Take(idxStart));
            newCode.AddRange(nodes);
            newCode.AddRange(code.CodeList.Skip(idxStart + len));

            var TA = new TACode();
            TA.CodeList = newCode;

            return TA;
        }

        /// <summary>
        /// Определение живых/мертвых переменных для участка кода
        /// </summary>
        /// <param name="listNode"></param>
        private void DetectionLiveAndDeadVariables(List<Node> listNode, Guid idx)
        {
            // Определение живых мертвых переменных для блока
            var LDV = new LiveAndDeadVariables(listNode);
            this.deadVars = LDV.DeadVars;
            this.liveVars = LDV.LiveVars;
            ActiveVar vars = OUT[idx];

            // Для каждой out переменной 
            foreach (var v in vars)
            {
                // Если она есть в списке мертвых переменных
                var dVars = deadVars.FindAll(x => x.Name == v);
                if (dVars.Count != 0)
                {
                    // Находим самую последнюю переменную
                    var max = dVars.Max(x => x.StringId);
                    var j = deadVars.FindIndex(x => x.Name == v && x.StringId == max);

                    // Перемещаем переменную в список живых переменных
                    liveVars.Add(new DUVar(deadVars[j].Name, deadVars[j].StringId));
                    deadVars.RemoveAt(j);
                }                
            }
        }

        /// <summary>
        /// Удаление мертвого кода в блоке
        /// </summary>
        private BasicBlock RemoveDeadCodeInBlock(BasicBlock Block)
        {
            var listNode = Block.CodeList.ToList();
            var idx = Block.BlockId;

            // Определяем живые/мертвые переменные
            DetectionLiveAndDeadVariables(listNode, idx);

            // Пока мы не удалим все мертвые переменные
            while (deadVars.Count != 0)
            {
                var i = 0;
                foreach (var dV in deadVars)
                    listNode.RemoveAt(dV.StringId - i++);

                deadVars.Clear();
                liveVars.Clear();

                // Определяем живые/мертвые переменные
                DetectionLiveAndDeadVariables(listNode, idx);
            }

            return new BasicBlock(listNode.ToList());
        }
    }
}
