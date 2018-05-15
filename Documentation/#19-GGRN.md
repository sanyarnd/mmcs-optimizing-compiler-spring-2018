### Название задачи
Общий алгоритм при наличии вектора оптимизаций (О1, О2, ...)

#### Постановка задачи
Необходимо реализовать алгоритм, который применял бы все возможные оптимизации для базового блока.

#### Зависимости задач в графе задач
Зависит от:
- Все реализованные оптимизации для базового блока

#### Теоретическая часть задачи
Требуется реализовать алгоритм, который применил бы к базовому блоку все имеющиеся оптимизации. Также необходимо, чтобы легко можно было добавить новые оптимизации.

#### Практическая часть задачи (реализация)
Был реализован класс `AllOptimizations` и функция `ApplyAllOptimizations`, применяющая все оптимизации к трехадресному коду.
```
public class AllOptimizations
	{
		private List<IOptimization> BasicBlockOptimizationList()
		{
			List<IOptimization> optimizations = new List<IOptimization>();

			optimizations.Add(new CopyPropagation());
			optimizations.Add(new ConstantFolding());
			optimizations.Add(new ConstantPropagation());
			optimizations.Add(new DeclarationOptimization());
			optimizations.Add(new AlgebraicOptimization());
			optimizations.Add(new SubexpressionOptimization());

			return optimizations;
		}

        private List<IOptimization> O2OptimizationList()
        {
            return new List<IOptimization>();
        }


        public TACode ApplyAllOptimizations(TACode code)
		{
			List<IOptimization> o1Optimizations = BasicBlockOptimizationList();
            var canApplyAny = true;

            while (canApplyAny)
            {
                canApplyAny = false;
                var blocks = code.CreateBasicBlockList().ToList();
                var codeList = new List<Node>();

                foreach (var b in blocks)
                {
                    var block = b.CodeList.ToList();
                    for (int i = 0; i < o1Optimizations.Count; i++)
                    {
                        block = o1Optimizations[i].Optimize(block, out var applied);
                        canApplyAny = canApplyAny || applied;
                    }
                    codeList.AddRange(block);
                }

                code = new TACode();
                code.CodeList = codeList;
            }

			return code;
		}
	}

```
#### Тесты
Короткие примеры, демонстрирующие правильность.

#### Пример работы.
Большой пример, демонстрирующий алгоритм.
