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
            _nodes.AppendLine($"{label}  [label = \"ASSIGN\"]");

            a.Id.Visit(this);
            a.Expr.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Id)}");
            _edges.AppendLine($"{label} -> {Mark(a.Expr)}");
        }

        public void VisitBinaryNode(BinaryNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"BINOP\"]");

            a.Left.Visit(this);
            a.Right.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Left)}");
            _edges.AppendLine($"{label} -> {Mark(a.Right)}");
        }

        public void VisitBlockNode(BlockNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"BLOCK\"]");

            foreach (var st in a.StList)
            {
                st.Visit(this);
                _edges.AppendLine($"{label} -> {Mark(st)}");
            }
        }

        public void VisitCycleNode(CycleNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"CYCLE\"]");

            a.Condition.Visit(this);
            a.Body.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Condition)}");
            _edges.AppendLine($"{label} -> {Mark(a.Body)}");
        }

        public void VisitEmptyNode(EmptyNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"EMPTY\"]");
        }

        public void VisitExprListNode(ExprListNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"EXPR_LIST\"]");

            foreach (var st in a.ExprList)
            {
                st.Visit(this);
                _edges.AppendLine($"{label} -> {Mark(st)}");
            }
        }

        public void VisitExprNode(ExprNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"EXPR\"]");
        }

        public void VisitForNode(ForNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"FOR\"]");

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
            _nodes.AppendLine($"{label}  [label = \"GOTO\"]");

            a.Label.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Label)}");
        }

        public void VisitIdNode(IdNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"ID: {a.Name}\"]");
        }

        public void VisitIfNode(IfNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"IF\"]");

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
            _nodes.AppendLine($"{label}  [label = \"INT: {a.Num}\"]");
        }

        public void VisitLabeledNode(LabeledNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"LABELED\"]");

            a.Label.Visit(this);
            a.Stat.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Label)}");
            _edges.AppendLine($"{label} -> {Mark(a.Stat)}");
        }

        public void VisitPrintNode(PrintNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"PRINT\"]");

            a.ExprList.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.ExprList)}");
        }

        public void VisitUnaryNode(UnaryNode a)
        {
            string label = Mark(a);
            _nodes.AppendLine($"{label}  [label = \"UNARY: {a.Operation}\"]");

            a.Num.Visit(this);

            _edges.AppendLine($"{label} -> {Mark(a.Num)}");
        }
    }
}
