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
    // Def цепочка 
    using DList = List<DNode>;
    // Use цепочка
    using UList = List<UNode>;

    /// <summary>
    /// Класс для построения Def и Use цепочек 
    /// в пределах одного блока
    /// </summary>
	public class DULists
	{
        /// <summary>
        /// Базовый блок
        /// </summary>
		public BasicBlock Block { get; }
        /// <summary>
        /// Def цепочка
        /// </summary>
        public DList DList { get; }
        /// <summary>
        /// Use цепочка
        /// </summary>
        public UList UList { get; }
        /// <summary>
        /// Use переменные, которые не определены в блоке
        /// </summary>
        public List<DUVar> UListNotValid { get; }
        /// <summary>
        /// Нумерация строк блока
        /// </summary>
        public Dictionary<Guid, int> N { get; }

        /// <summary>
        /// Конструктор для класса, генерирующий
        /// Def и Use цепочки по базовому блоку
        /// </summary>
        /// <param name="block">Базовый блок</param>
		public DULists(BasicBlock block)
		{
            this.Block = block;
            DList = new DList();
            UList = new UList();
            UListNotValid = new List<DUVar>();
            N = new Dictionary<Guid, int>();
            NumberOfStringInBlock();
            BuildDULists();
        }

        /// <summary>
        /// Конструктор для класса, генерирующий
        /// Def и Use цепочки по произвольному
        /// фрагменту кода
        /// </summary>
        /// <param name="nodes">Фрагмент кода</param>
		public DULists(List<Node> nodes) : this(new BasicBlock(nodes)) { }

        /// <summary>
        /// Создает Def и Use цепочки для базового блока
        /// </summary>
        private void BuildDULists()
        {
            BuildDList();
            BuildUList();
        }

        /// <summary>
        /// Нумерация строк блока
        /// </summary>
        private void NumberOfStringInBlock()
        {
            var i = 0;
            foreach (var command in Block.CodeList)
                N.Add(command.Label, i++);
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

                    // Добавление Use узлов
                    AddUseVariable(comA.Left, comA.Label);
                    AddUseVariable(comA.Right, comA.Label);

                    // Добавление нового Def узла
                    DList.Add(new DNode(comA.Result, N[comA.Label]));
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
                    return v.DefVariable.Name.Id == variable.Id;
                });

                var UVar = new DUVar(variable, N[strId]);

                // Добавление Use переменной
                if (index != -1)
                    DList[index].AddUseVariables(UVar);
                else
                    UListNotValid.Add(UVar);
            }
        }

        /// <summary>
        /// Создает Use цепочку для базового блока
        /// </summary>
        private void BuildUList()
        {
            foreach (var dN in DList)
            {
                var dVar = dN.DefVariable;
                
                foreach (var uVar in dN.UseVariables)
                {
                    UList.Add(new UNode(uVar as DUVar, dVar as DUVar));
                }
            }
        }

        public override string ToString()
        {
            return Block.ToString();
        }

        public override bool Equals(object obj)
        {
            return Block.Equals((obj as DULists)) &&
                DList.Equals((obj as DULists).DList) &&
                UList.Equals((obj as DULists).UList);
        }

        public override int GetHashCode()
        {
            return Block.GetHashCode() + DList.GetHashCode() 
                + UList.GetHashCode();
        }
    }

    /// <summary>
    /// Узел Def цепочки
    /// </summary>
    public class DNode
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
        /// <param name="DefVariable">Def переменная</param>
        /// <param name="UseVariables">Список Use переменных</param>
        public DNode(DUVar DefVariable, List<DUVar> UseVariables)
        {
            this.DefVariable = DefVariable;
            this.UseVariables = UseVariables.ToList();
        }

        /// <summary>
        /// Конструктор Def узла
        /// </summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="StringId">Идентификатор строки в блоке</param>
        public DNode(Var Name, int StringId)
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

        public static bool operator !=(DNode d1, DNode d2)
        {
            return !(d1 == d2);
        }

        public static bool operator ==(DNode d1, DNode d2)
        {
            var IsTrue = d1.UseVariables.Count == d2.UseVariables.Count;

            if (!IsTrue)
                return false;

            for (var i = 0; i < d1.UseVariables.Count; i++)
                IsTrue &= d1.UseVariables[i] == d2.UseVariables[i];

            return d1.DefVariable == d2.DefVariable && IsTrue;
        }

        public override string ToString()
        {
            return DefVariable.ToString();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as DNode);
        }

        public override int GetHashCode()
        {
            return DefVariable.GetHashCode() + UseVariables.GetHashCode();
        }
    }

    /// <summary>
    /// Узел Use цепочки
    /// </summary>
    public class UNode
    {
        /// <summary>
        /// Use переменная
        /// </summary>
		public DUVar UseVariable { get; }
        /// <summary>
        /// Def переменная
        /// </summary>
		public DUVar DefVariable { get; }

        /// <summary>
        /// Конструктор Use узла
        /// </summary>
        /// <param name="UseVariable">Use переменная</param>
        /// <param name="DefVariable">Def переменная</param>
        public UNode(DUVar UseVariable, DUVar DefVariable)
        {
            this.UseVariable = UseVariable;
            this.DefVariable = DefVariable;
        }

        public static bool operator !=(UNode u1, UNode u2)
        {
            return !(u1 == u2);
        }

        public static bool operator ==(UNode u1, UNode u2)
        {
            return u1.DefVariable == u2.DefVariable && 
                u1.UseVariable == u2.UseVariable;
        }

        public override string ToString()
        {
            return UseVariable.ToString();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as UNode);
        }

        public override int GetHashCode()
        {
            return DefVariable.GetHashCode() + UseVariable.GetHashCode();
        }
    }

    /// <summary>
    /// DefUse переменная 
    /// </summary>
	public class DUVar
    {
        /// <summary>
        /// Имя переменной
        /// </summary>
        public Var Name { get; }
        /// <summary>
        /// Идентификатор строки в блоке
        /// </summary>
		public int StringId { get; }

        /// <summary>
        /// Конструктор DefUse переменной
        /// </summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="StringId">Идентификатор строки в блоке</param>
        public DUVar(Var Name, int StringId) 
        {
            this.Name = Name;
            this.StringId = StringId;
        }

        public static bool operator !=(DUVar v1, DUVar v2)
        {
            return !(v1 == v2);
        }

        public static bool operator ==(DUVar v1, DUVar v2)
        {
            return v1.Name == v2.Name && v1.StringId == v2.StringId;
        }

        public override string ToString()
        {
            return base.ToString() + "; StringId = " 
                + StringId.ToString();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as DUVar);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + StringId.GetHashCode();
        }
    }
}
