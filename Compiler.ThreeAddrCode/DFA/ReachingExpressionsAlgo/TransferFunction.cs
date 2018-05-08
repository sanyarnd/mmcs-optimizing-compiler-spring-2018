using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.DFA.ReachingExpressionsAlgo
{
    public class TransferFunction : ITransferFunction<HashSet<Guid>>
    {
        /// <summary>
        /// Содержит узлы - выражения(присвоения), построенный по 3-х адрессному коду
        /// </summary>
        private List<Assign> AssignNodes;

        /// <summary>
        /// Конструктор для TransferFunction
        /// </summary>
        /// <param name="code">Необходим для списка выражений-присвоений</param>
        public TransferFunction(TACode code)
        {
            foreach (var node in code.CodeList)
            {
                if (node is Assign ass)
                {
                    if (ass.Left is Var || ass.Right is Var)
                        AssignNodes.Add(ass);
                }
            }
        }

        public HashSet<Guid> Transfer(BasicBlock basicBlock, HashSet<Guid> input, ILatticeOperations<HashSet<Guid>> ops)
        {
            //множество генерируемых выражений в блоке
            var e_gen = new HashSet<Guid>();

            //по определению множества e_gen, генерируемые выражения должны быть 
            //сгенерированы один раз и до конца блока не переопределяться. 
            //Введём вспомогательое множество, которое будет подсчитывать 
            //частоту появления выражения в блоке. Затем, отберём только те,
            //которые были замечены 1 раз
            var aux_dict_gen = new Dictionary<Guid, int>();
            foreach (var node in basicBlock.CodeList)
            {
                if (node is Assign ass)
                    if (ass.Left is Var || ass.Right is Var)
                        aux_dict_gen[ass.Label]++;
            }
            
            //Формирование множества e_gen
            foreach (var a in aux_dict_gen)
                if (a.Value == 1)
                    e_gen.Add(a.Key);

            //множество выражений, уничтожаемых блоком basicBlock
            var e_kill = new HashSet<Guid>();

            //Метки базового блока
            var bb_marks = new HashSet<Guid>(basicBlock.CodeList.Select(x => x.Label));
            foreach (var an in AssignNodes)
            {
                //Если левый операнд выражения содержится в базовом блоке,
                //то добавляем его в e_kill
                if (an.Left is Var lv)
                {
                    if (bb_marks.Contains(lv.Id)) 
                        e_kill.Add(an.Result.Id);
                }

                //Если правый операнд выражения содержится в базовом блоке,
                //то добавляем его в e_kill
                if (an.Right is Var rv)
                {
                    if (bb_marks.Contains(rv.Id))
                        e_kill.Add(an.Result.Id);
                }
            }
            var inset = new HashSet<Guid>(input);
            return new HashSet<Guid>(inset.Except(e_kill).Union(e_gen));
        }
    }
}
