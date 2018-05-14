### Название задачи
Свертка констант, протяжка констант

#### Постановка задачи
Создать класс, реализующий итерационный алгоритм для задачи распостронения констант, и класс, реализующий свертку констант.

#### Зависимости задач в графе задач
Зависит от:
- Трехадресный код

#### Теоретическая часть задачи
Распространение констант – хорошо известная проблема глобального анализа потока данных. Цель распространения констант состоит в обнаружении величин, которые являются постоянными при любом возможном пути выполнения программы, и в распространении этих величин так далеко по тексту программы, как только это возможно. Выражения, чьи операнды являются константами, могут быть вычислены на этапе компиляции. Поэтому использование алгоритмов распространения констант позволяет компилятору выдавать более компактный и быстрый код.

Рассмотрим следующий пример:
```
int x = 14;
int y = 7 - x / 2;
return y * (28 / x + 2);
```

Распространение x возвращает:
```
int x = 14;
int y = 7 - 14 / 2;
return y * (28 / 14 + 2);
```

Далее, свёртка констант и распространение y возвращают следующее:
```
int x = 14;
int y = 0;
return 0;
```

#### Практическая часть задачи (реализация)
Были реализованы классы `ConstantFolding` и `ConstantPropagation`.
```
public class ConstantFolding : IOptimization
	{
		private bool SetNode(Assign node)
		{
			switch (node.Operation)
			{
				case OpCode.Plus:
					node.Right = (IntConst)node.Left + (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Minus:
					node.Right = (IntConst)node.Left - (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Mul:
					node.Right = (IntConst)node.Left * (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				case OpCode.Div:
					node.Right = (IntConst)node.Left / (IntConst)node.Right;
					node.Operation = OpCode.Copy;
					node.Left = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return true;
		}
		public List<Node> Optimize(List<Node> nodes, out bool applied)
		{
			var app = false;
			var enumerable = nodes.OfType<Assign>().Where(assgn => assgn.Left is IntConst && assgn.Right is IntConst);

			foreach (var node in enumerable)
				app = SetNode(node);

			applied = app;
			return nodes;
		}
	}

```
```
public class ConstantPropagation : IOptimization
	{
		public List<Node> Optimize(List<Node> nodes, out bool applied)
		{
			var app = false;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i] is Assign node && node.Operation == OpCode.Copy && node.Right is IntConst)
				{
					for (int j = i + 1; j < nodes.Count; j++)
					{
						if (nodes[j] is Assign nextNode)
						{
							//Если мы встретили объявление этого же элемента
							if (node.Result.Equals(nextNode.Result))
								break;
							//Проверка использования Result в левом операнде другого узла
							if (node.Result.Equals(nextNode.Left))
							{
								nextNode.Left = node.Right;
								nodes[j] = nextNode;
								app = true;
							}
							//Проверка использования Result в правом операнде другого узла
							if (node.Result.Equals(nextNode.Right))
							{
								nextNode.Right = node.Right;
								nodes[j] = nextNode;
								app = true;
							}
						}
					}
				}
			}
			applied = app;
			return nodes;
		}
	}
```
#### Тесты
Короткие примеры, демонстрирующие правильность.

#### Пример работы.
Большой пример, демонстрирующий алгоритм.
