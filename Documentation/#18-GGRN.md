### Название задачи
Протяжка копий

#### Постановка задачи
Создать класс, реализующий алгоритм протяжки копий.

#### Зависимости задач в графе задач
Зависит от:
- Трехадресный код

От задачи зависит:
- Общий алгоритм при наличии вектора оптимизаций (О1, О2, ...)

#### Теоретическая часть задачи
Протяжка копий – еще одна известная проблема глобального анализа потока данных. Цель протяжки копий – заменить переменные их значениями.

Следующий фрагмент кода
```
y = x;
z = 3 + y 
```
после выполнения протяжки копий будет выглядеть вот так:
```
z = 3 + x
```

#### Практическая часть задачи (реализация)
Был реализован класс `CopyPropagation`.
```
public class CopyPropagation : IOptimization
	{
		public List<Node> Optimize(List<Node> nodes, out bool applied)
		{
			var app = false;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i] is Assign node && node.Operation == OpCode.Copy && !(node.Right is IntConst))
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
