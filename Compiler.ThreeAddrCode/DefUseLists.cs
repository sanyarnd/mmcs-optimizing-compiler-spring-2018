using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode
{
    /// <summary>
    /// Класс для построения Def и Use цепочек 
    /// в пределах одного блока
    /// </summary>
	class DULists
	{
        /// <summary>
        /// Базовый блок
        /// </summary>
		public BasicBlock Block { get; }
        /// <summary>
        /// Def цепочка
        /// </summary>
        public List<DNode> DList { get; }

        /// <summary>
        /// Конструктор для класса, генерирующий
        /// Def и Use цепочки по базовому блоку
        /// </summary>
        /// <param name="block">Базовый блок</param>
		public DULists(BasicBlock block)
		{
            this.Block = block;
            DList = new List<DNode>();
            BuildDList();
        }

        /// <summary>
        /// Создает Def цепочку для базового блока
        /// </summary>
        private void BuildDList()
        {
            var code = Block.CodeList;

            // Цикл по всем командам блока
            foreach (var command in code)
            { 
                if (command is Assign)
                {
                    var comA = command as Assign;
                    
                    // Добавление нового Def узла
                    if (comA.Result is Var)
                        DList.Add(new DNode(comA.Result.Id, comA.Label));

                    // Добавление Use узлов
                    AddUseVariable(comA.Left, comA.Label);
                    AddUseVariable(comA.Right, comA.Label);
                }
                else if (command is IfGoto)
                {
                    var comIG = command as IfGoto;
                    AddUseVariable(comIG.Condition, comIG.Label);
                }
                else if (command is Print)
                {
                    var comP = command as Print;
                    AddUseVariable(comP.Data, comP.Label);
                }
            }
        }

        /// <summary>
        /// Добавляет Use переменную в DList
        /// </summary>
        /// <param name="expr">Выражение</param>
        /// <param name="strId">Идентификатор строки в блоке</param>
        private void AddUseVariable(Expr expr, Guid strId)
        {
            if (expr is Var)
            {
                var variable = expr as Var;

                // Поиск последнего переопределения переменной
                var index = DList.FindLastIndex(v => 
                {
                    return v.DefVariable.Name == variable.Id;
                });

                var UVAr = new DUVar(variable.Id, strId);

                // Добавление Use переменной
                if (index != -1)
                    DList[index].AddUseVariables(UVAr);
                else
                    throw new Exception("Использование переменной до ее определения");
            }
        }

        public override string ToString()
        {
            return Block.ToString();
        }

        public override bool Equals(object obj)
        {
            return Block.Equals((obj as DULists)) &&
                DList.Equals((obj as DULists).DList);
        }

        public override int GetHashCode()
        {
            return Block.GetHashCode() + DList.GetHashCode();
        }
    }

    /// <summary>
    /// Узел Def цепочки
    /// </summary>
    class DNode
	{
        /// <summary>
        /// Def переменная
        /// </summary>
		public DUVar DefVariable { get; }
        /// <summary>
        /// Список Use переменных
        /// </summary>
		public List<DUVar> UseVariables { get; }

        /// <summary>
        /// Конструктор Def узла
        /// </summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="StringId">Идентификатор строки в блоке</param>
        public DNode(Guid Name, Guid StringId)
        {
            this.DefVariable = new DUVar(Name, StringId);
            this.UseVariables = new List<DUVar>();
        }

        /// <summary>
        /// Добавляет Use переменные
        /// </summary>
        /// <param name="UseVariables">Список используемых переменных</param>
        public void AddUseVariables(params DUVar[] UseVariables)
        {
            this.UseVariables.AddRange(UseVariables.ToList());
        }

        /// <summary>
        /// Удаляет Use переменные
        /// </summary>
        /// <param name="UseVariables">Список используемых переменных</param>
        public void RemoveUseVariables(params DUVar[] UseVariables)
        {
            for (var i = 0; i < this.UseVariables.Count; i++)
                if (UseVariables.Contains(this.UseVariables[i]))
                    this.UseVariables.RemoveAt(i--);
        }

        public override string ToString()
        {
            return DefVariable.ToString();
        }

        public override bool Equals(object obj)
        {
            return DefVariable.Equals((obj as DNode).DefVariable) &&
                UseVariables.Equals(((obj as DNode).UseVariables));
        }

        public override int GetHashCode()
        {
            return DefVariable.GetHashCode() + UseVariables.GetHashCode();
        }
    }

    /// <summary>
    /// DefUse переменная 
    /// </summary>
	class DUVar
	{
        /// <summary>
        /// Имя переменной
        /// </summary>
		public Guid Name { get; }
        /// <summary>
        /// Идентификатор строки в блоке
        /// </summary>
		public Guid StringId { get; }

        /// <summary>
        /// Конструктор DefUse переменной
        /// </summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="StringId">Идентификатор строки в блоке</param>
        public DUVar(Guid Name, Guid StringId)
		{
			this.Name = Name;
			this.StringId = StringId;
		}

        public override string ToString()
        {
            return "Name = " + Name.ToString() + "; StringId = " 
                + StringId.ToString();
        }

        public override bool Equals(object obj)
        {
            return Name.Equals((obj as DUVar).Name) &&
                StringId.Equals((obj as DUVar).StringId);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + StringId.GetHashCode();
        }
    }
}
