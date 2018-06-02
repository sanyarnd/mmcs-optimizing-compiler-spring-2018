### Название задачи  
Парсер языка и построение AST-дерева 

#### Постановка задачи  
Написать парсер языка на языке C# с использованием GPLex и Yacc. Реализовать построение синтаксического дерева программы.

#### Зависимости задач в графе задач
От задачи зависит:
* Базовые визиторы
* PrettyPrinter
* Генерация трёхадресного кода

#### Теоретическая часть задачи

Для автоматического создания парсера создаются файлы SimpleLex.lex (описание лексического анализатора) и SimpleYacc.y (описание синтаксического анализатора).
Код лексического и синтаксического анализаторов создаются на C# запуском командного файла generateParserScanner.bat.

Синтаксически управляемая трансляция состоит в том, что при разборе текста программы на каждое распознанное правило грамматики выполняется некоторое действие. Данные действия придают смысл трансляции (переводу) и поэтому мы называем их семантическими. Семантические действия записываются в .y-файле после правил в фигурных скобках и представляют собой код программы на C# (целевом языке компилятора).  

Как правило, при трансляции программа переводится в другую форму, более приспособленную для анализа, дальнейших преобразований и генерации кода.  

Мы будем переводить текст программы в так называемое синтаксическое дерево. Если синтаксическое дерево построено, то программа синтаксически правильная, и ее можно подвергать дальнейшей обработке.  

В синтаксическое дерево включаются узлы, соответствующие всем синтаксическим конструкциям языка. Атрибутами этих узлов являются их существенные характеристики. Например, для узла оператора присваивания AssignNode такими атрибутами являются IdNode - идентификатор в левой части оператора присваивания и ExprNode - выражение в правой части оператора присваивания.  

#### Практическая часть задачи (реализация)
#### Парсер языка
Парсер был реализован для языка со следующим синтаксисом:
```csharp
a = 777; // оператор присваивания
```
```csharp
// пример арифметических операций
a = a - b;
a = a + b;
a = a * 3;
a = 5 * b;
```
```csharp
// пример операторов сравнения
c = a < b;
c = b > a;
c = a != b;
c = a == b;
// логическое "нет"
c = !a;
```
```csharp
// полная форма условного оператора
if (a < b)
    a = 555;
else
{
    b = 666;
    с = 777;
}
// сокращенная форма условного оператора 
if (b == c)
    c = 666;
```
```csharp
// операторы циклов
// цикл while
while(3)
{
    ...
}
// цикл for c шагом 2
for(i = 0, 10, 2)
{
    ...
}
// цикл for c шагом 1 по умолчанию
for(i = 0, 10) 
{
    ...
}
```
```python
// оператор вывода
print(a);
print(a, b, c);
```
```csharp
// оператор goto
goto h;
// переход по метке 
h: {c = a + b;}
```

Для создания парсера использовались GPLex и Yacc, были созданы соответствующие файлы .lex и .y.  
Пример содержимого [.lex файла](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/SimpleLex.lex):
```csharp
{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
    yylval.sVal = yytext;
  return res;
}

"=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
```
Пример содержимого [.y файла](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/SimpleYacc.y): 
```csharp
%token <iVal> INUM 
%token <sVal> ID

%type <eVal> expr ident W T F 
%type <stVal> assign statement cycle for if 

expr : W  { $$ = $1; }
     | expr LT W { $$ = new BinaryNode($1, $3, OperationType.Less); }
     | expr GT W { $$ = new BinaryNode($1, $3, OperationType.Greater); }
     | expr LE W { $$ = new BinaryNode($1, $3, OperationType.LessEq); }
     | expr GE W { $$ = new BinaryNode($1, $3, OperationType.GreaterEq); }
     | expr EQ W { $$ = new BinaryNode($1, $3, OperationType.Equal); }
     | expr NEQ W { $$ = new BinaryNode($1, $3, OperationType.NotEqual); }
     ;
```
#### Построение AST-дерева
Для построения AST дерева были созданы классы для каждого типа узла:
* [Node.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/Node.cs) - базовый класс для всех узлов
* [ExprNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/ExprNode.cs) - базовый класс для выражений	
* [AssignNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/AssignNode.cs) - операция присваивания	 
* [BinaryNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/BinaryNode.cs) - класс для бинарных операций	  
* [UnaryNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/UnaryNode.cs) - класс для унарных операций
* [ExprListNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/ExprListNode.cs) - класс для списка операций	 
* [IntNumNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/IntNumNode.cs) - класс для целочисленных констант 	 
* [IdNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/IdNode.cs) - класс для идентификаторов	 

* [StatementNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/StatementNode.cs) - базовый класс для всех операторов
* [BlockNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/BlockNode.cs) - класс для блока
* [CycleNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/CycleNode.cs) - класс для цикла *while*	  
* [ForNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/ForNode.cs) - класс для цикла *for*	  	 
* [GoToNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/GoToNode.cs) - класс для *goto*	 
* [IfNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/IfNode.cs) - класс для оператора сравнения	 
* [LabeledNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/LabeledNode.cs) - класс метки goto
* [PrintNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/PrintNode.cs) - класс оператора вывода
* [EmptyNode.cs](https://github.com/Lucky112/mmcs-optimizing-compiler-spring-2018/blob/master/Compiler.Parser/AST/EmptyNode.cs) - класс для пустого узла

Пример кода, описывающего оператор вывода:
```csharp
namespace Compiler.Parser.AST
{
    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }

        public PrintNode(ExprListNode exprlist)
        {
            ExprList = exprlist;
        }
    }
}
```

#### Тесты
1. 
```cshap
b = 0;
```
![](https://image.ibb.co/kDW3MT/assign.png)
```csharp
if(a == 0)
   b = 0;
```
![](https://image.ibb.co/cbmET8/if1.png)
```csharp
for(a = 0, 10, 1) {
    x = 0;
}
```
![](https://image.ibb.co/gF5V1T/for.png)
#### Пример работы.
Алгоритм Евклида.
```csharp
a = 210;
b = 91;
 
while (a != b)
{
    if (a > b)
        a = a - b;
    else
        b = b - a;
}
print (a);
```
![](https://image.ibb.co/hrMRao/ast.png)


