using Compiler.ThreeAddrCode.CFG;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ThreeAddrCode.DominatorTree
{
    public class DominatorTree
    {
        private List<DomRow> _matrix { get; set; } = new List<DomRow>();
        public List<DomRow> Matrix { get => _matrix; }

        /*
         * Формула построение дерева доминаторов
         * dom(x) = {x} || { dom(i_1) && dom(i_2) && ... && dom(i_k) }
         * x — узел (не root)
         * i_j — предок узла х
         */


        public DominatorTree() => _matrix = new List<DomRow>();


        /// <summary>
        ///     Создаёт матрицу доминаторов. Возвращает матрицу, значения которой говорят нам, доминирует ли блок j над блоком i.
        /// </summary>
        public void CreateDomMatrix(ControlFlowGraph CFG)
        {
            // Инициализируем переменные
            int N = CFG.CFGNodes.Count;

            // Запонлняем матрицу смежности для дерева доминаторов единицами
            foreach (var bb in CFG.CFGNodes)
            {
                var item = new DomRow
                {
                    BasicBlock = bb
                };
                foreach (var bb1 in CFG.CFGNodes)
                {
                    item.ItemDoms.Add(new DomCell
                    {
                        BasicBlock = bb1,
                        HasLine = true
                    });
                }
                _matrix.Add(item);
            }

            // По правилу все ячейки должны быть 1, кроме 1 строки в промежутке от 2ой до последней ячейки
            // Пример
            /* [
             * 1 0 0 0,
             * 1 1 1 1,
             * 1 1 1 1,
             * 1 1 1 1,
             * ]
             */
            for (int i = 1; i < N; i++)
            {
                _matrix[0].ItemDoms[i].HasLine = false;
            }

            // Считаем матрицу смежности для дерева доминаторов
            for (int i = 1; i < N; i++)
            {
                // Заполняем значение {x} в формуле. {x} является узел, который доминирует только сам над собой
                List<DomCell> blockRow = new List<DomCell>(N);
                foreach (var bb in CFG.CFGNodes)
                {
                    // Если id ББ-ка совпадает с текущим, ставим 1 в противном случае 0.
                    blockRow.Add(new DomCell
                    {
                        BasicBlock = bb,
                        HasLine = bb.BlockId == CFG.CFGNodes[i].BlockId
                    });
                }

                // Список = { dom(i_1) && dom(i_2) && ... && dom(i_k) }
                List<bool> results = new List<bool>(N);
                results.ForEach(x => x = true);

                foreach (var parent in CFG.CFGNodes[i].Parents)
                {
                    // Находим dom каждого предка
                    var domParent = _matrix.Single(row => row.BasicBlock.BlockId == parent.BlockId);
                    for (int j = 0; j < N; j++)
                    {
                        // Поэлементно находим конъюнкцию всех предков
                        results[j] &= domParent.ItemDoms[j].HasLine;
                    }
                }

                // Получили dom(i) = {i} || results. Следовательно выполняем поэлементную дизъюнкцию
                for (var j = 0; j < N; j++)
                {
                    // Выбираем строку с текущим блоком, и меняем все его значения
                    _matrix[i].ItemDoms[j].HasLine = blockRow[j].HasLine || results[j];
                }
            }
        }

    }
}
