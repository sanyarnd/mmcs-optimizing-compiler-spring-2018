using System.Collections.Generic;
using Compiler.Parser.Visitors;

namespace Compiler.Parser.AST
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
    }

    public class BinaryNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public string Operation { get; set; }
        public BinaryNode(ExprNode left, ExprNode right, string op)
        {
            Left = left;
            Right = right;
            Operation = op;
        }
        public override void Visit(Visitor v)
        {
            v.VisitBinaryNode(this);
        }
    }

    public class UnaryNode : ExprNode
    {
        public ExprNode Num { get; set; }
        public char Operation { get; set; }
        public UnaryNode(ExprNode num, char op)
        {
            Num = num;
            Operation = op;
        }
        public UnaryNode(int num, char op) : this(new IntNumNode(num), op) {}
        public override void Visit(Visitor v)
        {
            v.VisitUnaryNode(this);
        }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class LabelNode : StatementNode
    {
        public StatementNode Stat { get; set; }
        public IdNode Label { get; set; }

        public LabelNode(IdNode label, StatementNode stat)
        {
            Label = label;
            Stat = stat;
        }

        public override void Visit(Visitor v)
        {
            v.VisitLabelNode(this);
        }
    }

    public class GoToNode : StatementNode
    {
        public IdNode Label { get; set; }

        public GoToNode(IdNode label)
        {
            Label = label;
        }

        public override void Visit(Visitor v)
        {
            v.VisitGoToNode(this);
        }
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
        public override void Visit(Visitor v)
        {
            v.VisitCycleNode(this);
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }
        public PrintNode(ExprListNode exprlist)
        {
            ExprList = exprlist;
        }
        public override void Visit(Visitor v)
        {
            v.VisitPrintNode(this);
        }
    }

    public class ExprListNode : Node
    {
        public List<ExprNode> ExpList = new List<ExprNode>();
        public ExprListNode(ExprNode exp)
        {
            Add(exp);
        }
        public void Add(ExprNode exp)
        {
            ExpList.Add(exp);
        }
        public override void Visit(Visitor v)
        {
            v.VisitExprListNode(this);
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat1 { get; set; }
        public StatementNode Stat2 { get; set; }
        public IfNode(ExprNode expr, StatementNode stat1, StatementNode stat2)
        {
            Expr = expr;
            Stat1 = stat1;
            Stat2 = stat2;
        }

        public IfNode(ExprNode expr, StatementNode stat1)
        {
            Expr = expr;
            Stat1 = stat1;
            Stat2 = null;
        }
        public override void Visit(Visitor v)
        {
            v.VisitIfNode(this);
        }
    }

    public class ForNode : StatementNode
    {
        public AssignNode Assign { get; set; }
        public ExprNode Cond { get; set; }
        public ExprNode Inc { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(AssignNode assign, ExprNode cond, ExprNode inc, StatementNode stat)
        {
            Assign = assign;
            Cond = cond;
            Inc = inc;
            Stat = stat;
        }

        public ForNode(AssignNode assign, ExprNode cond, StatementNode stat): this(assign, cond, null, stat) {}

        public override void Visit(Visitor v)
        {
            v.VisitForNode(this);
        }
    }

    public class EmptyNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitEmptyNode(this);
        }
    }


}