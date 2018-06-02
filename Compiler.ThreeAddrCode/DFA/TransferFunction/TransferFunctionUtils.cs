using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.DFA;

namespace Compiler.ThreeAddrCode.DFA.TransferFunction
{
    /// <summary>
    /// Transfer function utils.
    /// </summary>
    public static class TransferFunctionUtils
    {
        /// <summary>
        /// Композиция функций f1 и f2.
        /// </summary>
        /// <returns>OUT множество композиции</returns>
        /// <param name="f1">Первая функция</param>
        /// <param name="f2">Вторая функцияD</param>
        /// <typeparam name="T">Тип передаточной функции</typeparam>
        public static ITransferFunction<T> Compose<T>(this ITransferFunction<T> f1, ITransferFunction<T> f2)
        {
            return new TransferFuctionComposition<T>(f1, f2);
        }

        /// <summary>
        /// Класс композиции функций
        /// </summary>
        /// <typeparam name="T">Compose type.</typeparam>
        private class TransferFuctionComposition<T> : ITransferFunction<T>
        {
            /// <summary>
            /// Функции-аргументы композиции
            /// </summary>
            private readonly ITransferFunction<T> f1, f2;

            /// <summary>
            /// Конструктор класса
            /// </summary>
            /// <param name="f1">Первая функция</param>
            /// <param name="f2">Вторая функция</param>
            public TransferFuctionComposition(ITransferFunction<T> f1, ITransferFunction<T> f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }

            /// <summary>
            /// Передаточная функция
            /// </summary>
            /// <returns>OUT множество</returns>
            /// <param name="basicBlock">базовый блок</param>
            /// <param name="input">IN множество</param>
            /// <param name="ops">набор операций над решеткой</param>
            public T Transfer(BasicBlock basicBlock, T input, ILatticeOperations<T> ops)
            {
                return this.f2.Transfer(basicBlock, this.f1.Transfer(basicBlock, input, ops), ops);
            }
        }
    }
}
