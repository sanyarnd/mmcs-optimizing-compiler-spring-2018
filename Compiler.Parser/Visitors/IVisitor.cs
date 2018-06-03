using Compiler.Parser.AST;

namespace Compiler.Parser.Visitors
{
    /// <summary>
    ///     Интерфейс визитора
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// Посетить ID узел
        /// </summary>
        /// <param name="id"></param>
        void VisitIdNode(IdNode id);

        /// <summary>
        /// Посетить INT узел
        /// </summary>
        /// <param name="num"></param>
        void VisitIntNumNode(IntNumNode num);

        /// <summary>
        /// Посетить EXPR узел
        /// </summary>
        /// <param name="node"></param>
        void VisitExprNode(ExprNode node);

        /// <summary>
        /// Посетить BINOP узел
        /// </summary>
        /// <param name="binop"></param>
        void VisitBinaryNode(BinaryNode binop);

        /// <summary>
        /// Посетить UNOP узел
        /// </summary>
        /// <param name="unop"></param>
        void VisitUnaryNode(UnaryNode unop);

        /// <summary>
        /// Посетить LABELED узел
        /// </summary>
        /// <param name="l"></param>
        void VisitLabeledNode(LabeledNode l);

        /// <summary>
        /// Посетить GOTO узел
        /// </summary>
        /// <param name="g"></param>
        void VisitGoToNode(GoToNode g);

        /// <summary>
        /// Посетить ASSIGN узел
        /// </summary>
        /// <param name="a"></param>
        void VisitAssignNode(AssignNode a);

        /// <summary>
        /// Посетить CYCLE узел
        /// </summary>
        /// <param name="c"></param>
        void VisitCycleNode(CycleNode c);

        /// <summary>
        /// Посетить BLOCK узел
        /// </summary>
        /// <param name="bl"></param>
        void VisitBlockNode(BlockNode bl);

        /// <summary>
        /// Посетить PRINT узел
        /// </summary>
        /// <param name="p"></param>
        void VisitPrintNode(PrintNode p);

        /// <summary>
        /// Посетить EXPR_LIST узел
        /// </summary>
        /// <param name="el"></param>
        void VisitExprListNode(ExprListNode el);

        /// <summary>
        /// Посетить IF узел
        /// </summary>
        /// <param name="iif"></param>
        void VisitIfNode(IfNode iif);

        /// <summary>
        /// Посетить FOR узел
        /// </summary>
        /// <param name="w"></param>
        void VisitForNode(ForNode w);

        /// <summary>
        /// Посетить EMPTY узел
        /// </summary>
        /// <param name="w"></param>
        void VisitEmptyNode(EmptyNode w);

    }
}