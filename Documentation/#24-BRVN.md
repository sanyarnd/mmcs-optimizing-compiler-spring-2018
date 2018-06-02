### Название задачи
Передаточная функция для достигающих определений, множества gen и kill.

#### Постановка задачи
Реализовать передаточную функция для достигающих определений, вычисление множеств gen и kill.

#### Зависимости задач в графе задач
Зависит от:
* Структура базовых блоков
* Интерфейс передаточной функции

#### Теоретическая часть задачи
 **Определение:** Будем говорить, что определение d достигает точки p, если существует путь от точки, непосредственно следующей за d, к точке p, такой, что d не уничтожается вдоль этого пути.  
Привести пример
Анализ должен быть консервативным: если не знаем, есть ли другое присваивание на пути, то считаем, что существует.  
Достигающие определения используются при:  
1. Является ли x константой в точке p? (если p достигает одно
определение x, и это – определение константы)  
2. Является ли x в точке p неинициализированной? (если p не
достигает ни одно определение x)  

**genB** – множество определений, генерируемых базовым блоком B.  
**killB** – множество остальных определений переменных, определяемых в определениях genB, в других ББл.

#### Практическая часть задачи (реализация)
Ниже представлена реализация передаточной функции для достигающих определений и метод вычисления множеств gen и kill.
```csharp
 public class TransferFunction : ITransferFunction<HashSet<Guid>>
    {
        private TACode taCode;

        public TransferFunction(TACode ta) => taCode = ta;

        public HashSet<Guid> Transfer(BasicBlock basicBlock, HashSet<Guid> input, ILatticeOperations<HashSet<Guid>> ops)
        {
            var (gen, kill) = GetGenAndKill(basicBlock, ops);
            var inset = new HashSet<Guid>(input);
            return new HashSet<Guid>(inset.Except(kill).Union(gen));
        }

        public (HashSet<Guid>, HashSet<Guid>) GetGenAndKill (BasicBlock basicBlock, ILatticeOperations<HashSet<Guid>> ops)
        {
            var gen = new HashSet<Guid>(basicBlock.CodeList.Where(x => x is Assign).Select(x => x.Label));
            var vars = basicBlock.CodeList
                .Where(x => x is Assign)
                .Select(x => ((x as Assign).Result as Var).Id)
                .ToList();
            var ad = taCode.CodeList
                .Where(x => !gen.Contains(x.Label) && x is Assign)
                .Cast<Assign>()
                .Where(x => vars.Contains((x.Result as Var).Id))
                .Select(x => x.Label);
            var kill = new HashSet<Guid>(ad);
            return (gen, kill);
        }
    }
```

#### Тесты
```csharp
l1: a = 3 - 5
l2: b = 10 + 2
l3: c = -1
l4: if 1 goto l3
l5: d = c + 1999
l6: if 2 goto l2
l7: e = 7 * 4
l8: f = 100 / 25
```
```csharp
gen(B1) = { l1 }
kill(B1) = { }

gen(B2) = { l2 }
kill(B2) = { }

gen(B3) = { l3 }
kill(B3) = { }

gen(B4) = { l5 }
kill(B4) = { }

gen(B5) = { l7, l8 }
kill(B4) = { }
```

#### Пример работы.
![](https://image.ibb.co/hjczy8/Capture.png)


