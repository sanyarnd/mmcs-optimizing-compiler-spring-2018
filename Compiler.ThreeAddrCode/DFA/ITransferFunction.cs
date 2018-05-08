using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA
{
    /// <summary>
    ///     Интерфейс передаточной функции
    /// </summary>
    /// <typeparam name="T">тип IN/OUT множества</typeparam>
    public interface ITransferFunction<T>
    {
        /// <summary>
        ///     Главный метод передаточной функции
        /// </summary>
        /// <param name="basicBlock">базовый блок</param>
        /// <param name="input">IN множество</param>
        /// <param name="ops">набор операций над решеткой</param>
        /// <returns>OUT множество</returns>
        T Transfer(BasicBlock basicBlock, T input, ILatticeOperations<T> ops);
    }
}