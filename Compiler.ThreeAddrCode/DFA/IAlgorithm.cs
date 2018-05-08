using Compiler.ThreeAddrCode.CFG;

 
namespace Compiler.ThreeAddrCode.DFA
{
    /// <summary>
    ///     Интерфейс DFA-алгоритма
    /// </summary>
    /// <typeparam name="T">тип IN/OUT множества</typeparam>
    public interface IAlgorithm<T>
    {
        /// <summary>
        ///     Главный метод DFA-алгоритма
        /// </summary>
        /// <param name="graph">граф потока управления</param>
        /// <param name="ops">операции, связанные с решеткой</param>
        /// <param name="f">передаточная функция</param>
        /// <returns>словарь вида { ББл: (IN, OUT) }</returns>
        InOutData<T> Analyze(ControlFlowGraph graph, ILatticeOperations<T> ops, ITransferFunction<T> f);
    }
}