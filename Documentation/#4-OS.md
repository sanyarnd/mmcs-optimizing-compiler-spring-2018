### Название задачи

Базовые визиторы и изменения в узлах AST-дерева.

#### Постановка задачи

Описать визиторы для обхода всех вершин синтаксического дерева и базовую логику обхода подузлов

#### Зависимости задач в графе задач
* Парсер
* Синтаксическое дерево

#### Теоретическая часть задачи

Для работы с синтаксическим деревом требуется метод его обхода, для этого реализуется интерфейс визитора.

Паттерн **Visitor** позволяет выполнить над каждым объектом некоторой структуры операцию, не загрязняя код класса этого объекта и не используя определение типа для каждого объекта.

#### Практическая часть задачи (реализация)

Реализован абстрактный класс *Visitor* с методами для обхода каждого типа узла
```
 public abstract class Visitor
    {
        public virtual void VisitIdNode(IdNode id) { }
        public virtual void VisitIntNumNode(IntNumNode num) { }
        public virtual void VisitBinaryNode(BinaryNode binop) { }
        public virtual void VisitUnaryNode(UnaryNode unop) { }
        public virtual void VisitLabelNode(LabelNode l) { }
        public virtual void VisitGoToNode(GoToNode g) { }
        public virtual void VisitAssignNode(AssignNode a) { }
        public virtual void VisitCycleNode(CycleNode c) { }
        public virtual void VisitBlockNode(BlockNode bl) { }
        public virtual void VisitPrintNode(PrintNode p) { }
        public virtual void VisitExprListNode(ExprListNode el) { }
        public virtual void VisitIfNode(IfNode iif) { }
        public virtual void VisitForNode(ForNode w) { }
        public virtual void VisitEmptyNode(EmptyNode w) { }
    }
```

А так же в базовом классе для всех узлов определен абстрактный метод *Visit*, которому передается текущий визитор в качестве параметра

```
    public abstract class Node // базовый класс для всех узлов    
     {
        public abstract void Visit(Visitor v);
     }
```

и в каждом узле синтаксического дерева, который требуется обходить, переопределен метод Visit:

```
    public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
```

Так же реализован класс для обхода потомков - *AutoVisitor*

```
 public class AutoVisitor : Visitor
    {
        public override void VisitBinaryNode(BinaryNode binop)
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }
        public override void VisitUnaryNode(UnaryNode unop)
        {
            unop.Num.Visit(this);
        }
        public override void VisitLabelNode(LabelNode l)
        {
            l.Label.Visit(this);
            l.Stat.Visit(this);
        }
    .....................   
        public override void VisitExprListNode(ExprListNode exn)
        {
            foreach (ExprNode ex in exn.ExpList)
                ex.Visit(this);
        }
        public override void VisitIfNode(IfNode iif)
        {
            iif.Expr.Visit(this);
            iif.Stat1.Visit(this);
            iif.Stat2?.Visit(this);
        }
        public override void VisitForNode(ForNode f)
        {
            f.Assign.Visit(this);
            f.Cond.Visit(this);
            f.Inc?.Visit(this);
            f.Stat.Visit(this);
        }
    }
```