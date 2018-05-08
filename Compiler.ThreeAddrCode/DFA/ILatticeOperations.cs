using System;

namespace Compiler.ThreeAddrCode.DFA
{
    /// <summary>
    ///     Интерфейс набора, определяющего логику и поведение DFA-алгоритмов.
    ///     Включает в себя оператор /\, оператор сравнения, а так же верхнюю и нижнюю границы 
    /// </summary>
    public interface ILatticeOperations<T>
    {
        /// <summary>
        ///     Оператор /\
        /// </summary>
        /// <param name="a">левый операнд</param>
        /// <param name="b">правый операнд</param>
        /// <returns>результат выполнения оператора (пересечение, объединение и т.п.)</returns>
        T Operator(T a, T b);

        /// <summary>
        ///     Бинарный оператор множества, который в некоторых случаях
        ///     может быть не определен и должен возвращать null
        /// </summary>
        /// <param name="a">левый операнд</param>
        /// <param name="b">правый операнд</param>
        /// <returns>true, если a LEQ b, иначе false; если сравнение недопустимо, то возвращается null</returns>
        bool? Compare(T a, T b);

        /// <summary>
        ///     Нижняя граница
        /// </summary>
        T Lower { get; }

        /// <summary>
        ///     Верхняя граница
        /// </summary>
        T Upper { get; }
    }
}