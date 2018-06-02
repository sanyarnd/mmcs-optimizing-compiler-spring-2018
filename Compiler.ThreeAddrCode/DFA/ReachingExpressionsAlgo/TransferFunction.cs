using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.DFA.ReachingExpressions
{
    public class TransferFunction : ITransferFunction<HashSet<Guid>>
    {
        /// <summary>
        /// Содержит узлы - выражения(присвоения), построенный по 3-х адрессному коду
        /// Выражения вида: z = x + y
        /// </summary>
        private List<Assign> AssignNodes;

        /// <summary>
        /// Конструктор для TransferFunction
        /// </summary>
        /// <param name="code">Необходим для списка выражений-присвоений</param>
        public TransferFunction(TACode code)
        {
            AssignNodes = new List<Assign>();
            foreach (var node in code.CodeList)
            {
                if ((node is Assign ass) && (ass.Left is Var || ass.Right is Var))
                {
                    AssignNodes.Add(ass);
                }
            }
        }

        public (HashSet<Guid>, HashSet<Guid>) GetEGenEKill(BasicBlock basicBlock)
        {
            //множество генерируемых выражений в блоке
            var e_gen = new HashSet<Guid>();

            //по определению множества e_gen, генерируемые выражения должны быть 
            //сгенерированы один раз и до конца блока не переопределяться. 

            //Формирование множества e_gen
            int node_index = 0;
            foreach (var n in basicBlock.CodeList)
            {
                if ((n is Assign ass) && ((ass.Left is Var) || (ass.Right is Var)))
                {
                    bool redefinition_exists = false;
                    //пропускаем node_index + 1 элементов и просматриваем до конца блока
                    // на наличие присвоений переменных из текущего выражения
                    foreach (var nn in basicBlock.CodeList.Skip(node_index + 1))
                    {
                        if (nn is Assign ass1)
                        {
                            if ((ass.Left is Var lv) && (lv.Id == ass1.Result.Id))
                            {
                                redefinition_exists = true;
                                break;
                            }

                            if ((ass.Right is Var rv) && (rv.Id == ass1.Result.Id))
                            {
                                redefinition_exists = true;
                                break;
                            }
                        }
                    }
                    if (!redefinition_exists)
                        e_gen.Add(ass.Label);
                }
                node_index++;
            }

            //множество выражений, уничтожаемых блоком basicBlock
            var tmp_e_kill = new HashSet<Guid>();

            /******************Поиск выражений, которые убивает базовый блок****************/
            //Узлы-выражения для базового блока
            List<Assign> bbl_assign_nodes = new List<Assign>();
            foreach (var n in basicBlock.CodeList)
            {
                if (n is Assign ass)
                    bbl_assign_nodes.Add(ass);
            }

            //AssignNodes, в котором нет выражений базового блока
            List<Assign> excepted_assign_nodes = AssignNodes.Except(bbl_assign_nodes).ToList();

            //метки результатов-присвоений выражений базового блока
            List<Guid> marks_bbl_an = bbl_assign_nodes.Select(x => x.Result.Id).ToList();

            foreach (var ean in excepted_assign_nodes)
            {
                bool contains = false;
                if ((ean.Left is Var lv) && marks_bbl_an.Contains(lv.Id))
                    contains = true;
                if (!contains && (ean.Right is Var rv) && marks_bbl_an.Contains(rv.Id))
                    contains = true;
                if (contains)
                    tmp_e_kill.Add(ean.Label);
            }

            /***************Поиск e_kill множества в самом блоке************/
            var all_bbl_assigns = new HashSet<Guid>();
            foreach (var n in basicBlock.CodeList)
                if ((n is Assign ass) && (ass.Left is Var || ass.Right is Var))
                    all_bbl_assigns.Add(ass.Label);

            //Множество e_kill в самом блоке
            var local_e_kill = new HashSet<Guid>(all_bbl_assigns.Except(e_gen));

            //Объединение локального e_kill и общего
            var e_kill = new HashSet<Guid>(tmp_e_kill.Union(local_e_kill));

            return (e_gen, e_kill);
        }

        public HashSet<Guid> Transfer(BasicBlock basicBlock, HashSet<Guid> input, ILatticeOperations<HashSet<Guid>> ops)
        {
            var (e_gen, e_kill) = GetEGenEKill(basicBlock);         
            var inset = new HashSet<Guid>(input);
            return new HashSet<Guid>(inset.Except(e_kill).Union(e_gen));
        }
    }
}
