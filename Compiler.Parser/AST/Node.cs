using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    /// <summary>
    ///     Базовый класс для всех узлов
    /// </summary>
    public abstract class Node
    {
        public abstract void Visit(IVisitor v);
    }
}