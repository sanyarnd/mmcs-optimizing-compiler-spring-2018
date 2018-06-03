using Compiler.Parser.AST;
using Compiler.Parser.Visitors;
using System.Runtime.Serialization;
using System.Text;

namespace Compiler.IDE.Handlers
{
    internal class AstGraphvizVisitor : IVisitor
    {
        private readonly ObjectIDGenerator _gen = new ObjectIDGenerator();

        public string Nodes => _nodes.ToString();
        private readonly StringBuilder _nodes = new StringBuilder();

        public string Edges => _edges.ToString();
        private readonly StringBuilder _edges = new StringBuilder();

        private string Mark(Node node)
        {
            return _gen.GetId(node, out _).ToString();
        }

        public void VisitAssignNode(AssignNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Assign: {a.Id.Name} = expr\"]");

            a.Id.Visit(this);
            a.Expr.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Id)}");
            _edges.AppendLine($"{label} -> {Mark(a.Expr)}");
        }

        public void VisitBinaryNode(BinaryNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"BinOp: {a.Operation}\"]");

            a.Left.Visit(this);
            a.Right.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Left)}");
            _edges.AppendLine($"{label} -> {Mark(a.Right)}");
        }

        public void VisitBlockNode(BlockNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Block\"]");

            foreach (var st in a.StList)
            {
                st.Visit(this);
                _edges.AppendLine($"{label} -> {Mark(st)}");
            }
        }

        public void VisitCycleNode(CycleNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Cycle\"]");

            a.Condition.Visit(this);
            a.Body.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Condition)}");
            _edges.AppendLine($"{label} -> {Mark(a.Body)}");
        }

        public void VisitEmptyNode(EmptyNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Empty\"]");
        }

        public void VisitExprListNode(ExprListNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"ExprList\"]");

            foreach (var st in a.ExprList)
            {
                st.Visit(this);
                _edges.AppendLine($"{label} -> {Mark(st)}");
            }
        }

        public void VisitExprNode(ExprNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Expr\"]");
        }

        public void VisitForNode(ForNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"For: {a.Assign.Id.Name} = expr; border; inc; body\"]");

            a.Assign.Visit(this);
            a.Border.Visit(this);
            a.Inc.Visit(this);
            a.Body.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Assign)}");
            _edges.AppendLine($"{label} -> {Mark(a.Border)}");
            _edges.AppendLine($"{label} -> {Mark(a.Inc)}");
            _edges.AppendLine($"{label} -> {Mark(a.Body)}");
        }

        public void VisitGoToNode(GoToNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"GoTo: {a.Label.Name}\"]");

            a.Label.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Label)}");
        }

        public void VisitIdNode(IdNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Id: {a.Name}\"]");
        }

        public void VisitIfNode(IfNode a)
        {
            string label = Mark(a);

            string elsePresent = "present";
            if (a.ElseClause == null)
                elsePresent = elsePresent.Insert(0, "not ");
            _nodes.AppendLine($"{label}  [label = \"If: else is {elsePresent} \"]");

            a.Conditon.Visit(this);
            a.IfClause.Visit(this);
            a.ElseClause?.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Conditon)}");
            _edges.AppendLine($"{label} -> {Mark(a.IfClause)}");
            if (a.ElseClause != null)
                _edges.AppendLine($"{label} -> {Mark(a.ElseClause)}");
        }

        public void VisitIntNumNode(IntNumNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Int: {a.Num}\"]");
        }

        public void VisitLabeledNode(LabeledNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Labeled: {a.Label.Name}, statement\"]");

            a.Label.Visit(this);
            a.Stat.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Label)}");
            _edges.AppendLine($"{label} -> {Mark(a.Stat)}");
        }

        public void VisitPrintNode(PrintNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"Print: exprList\"]");

            a.ExprList.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.ExprList)}");
        }

        public void VisitUnaryNode(UnaryNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"UnaryOp: {a.Operation}, {a.Num} \"]");

            a.Num.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Num)}");
        }
    }
}