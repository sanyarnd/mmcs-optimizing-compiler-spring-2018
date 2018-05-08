using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA.ReachingExpressions
{
    /// <summary>
    ///     Класс, осуществляющий применение результата работы итеративного алгоритма для доступных выражений к
    ///     всему множеству блоков ТА кода (графу потока управления).
    /// </summary>
    /// <param name="cfg">Граф потока управления</param>
    /// <param name="re">Доступные выражения после проведения анализа</param>
    /// <returns>Граф потока управления с измененными IN/OUT множествами</returns>
    public class ExpressionsCleaner
    {
        private ControlFlowGraph cfg;
        private ReachingExpressions re;

        public ExpressionsCleaner(ControlFlowGraph cfg, ReachingExpressions re)
        {
            this.cfg = cfg;
            this.re = re;
        }

        /// <summary>
        ///     Заменяет исходное IN/OUT множество на оптимизированное.
        /// </summary>
        public void Clean()
        {
            foreach (var node in cfg.CFGNodes)
            {
                node.In = re.In;
                node.Out = re.Out;
            }
        }
    }
}