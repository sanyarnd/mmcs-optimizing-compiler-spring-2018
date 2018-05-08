using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA.ReachingExpressionsAlgo
{
    /// <summary>
    /// Реализация итеративного алгоритма для достигающих выражений
    /// Реализация взята с ReachingDefinitions с небольшими изменениями
    /// </summary>
    public class IterativeAlgorithm : IAlgorithm<HashSet<Guid>>
    {
        public InOutData<HashSet<Guid>> Analyze(ControlFlowGraph graph, ILatticeOperations<HashSet<Guid>> ops, ITransferFunction<HashSet<Guid>> f)
        {
            var data = new InOutData<HashSet<Guid>>
            {
                //Инициализация для первого ББл пустым множеством - нижней границей
                [graph.CFGNodes.ElementAt(0)] = (
                    ops.Lower,
                    f.Transfer(graph.CFGNodes.ElementAt(0), ops.Lower, ops)
                )
            };

            //Инициализация всех блоков, кроме первого универсальным множеством
            foreach (var node in graph.CFGNodes.Skip(1))
                data[node] = (ops.Upper, ops.Upper);

            var outChanged = true;

            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.CFGNodes.Skip(1))
                {
                    //выполняем сбор выражений посредством операции пересечения 
                    var inset = block.Parents.Aggregate(ops.Upper, (x, y)
                        => ops.Operator(x, data[y].Item2));

                    //Выполнение передаточной функции
                    var outset = f.Transfer(block, inset, ops);

                    //Проверка на изменения в множестве outset
                    if (outset.Except(data[block].Item2).Count() != 0)
                    {
                        outChanged = true;
                        data[block] = (inset, outset);
                    }
                }
            }
            return data;
        }
    }
}
