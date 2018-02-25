﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ThreeAddressCode
{
    /*
     * =========================================================
     * Трехадресный код имеет следующий вид:
     * Result := Left Operation Right
     * Result, Left, Right - операнды
     * Operation - бинарная операция 
     * =========================================================
    */

    public enum OpCode
    {
        TA_Plus,
        TA_Minus,
        TA_Mul,
        TA_Div,

        TA_UnaryMinus,
        TA_Copy
    }

    public static class OpCodeExt
    {
        public static string GetSymbol(this OpCode op)
        {
            switch (op)
            { 
                case OpCode.TA_Plus: return "+";
                case OpCode.TA_UnaryMinus: 
                case OpCode.TA_Minus: return "-";
                case OpCode.TA_Mul: return "*";
                case OpCode.TA_Div: return "/";
                case OpCode.TA_Copy: return "";

                default: return "unknown";
            }
        }
    }

    /// <summary>
    /// Базовый узел
    /// </summary>
    public class TA_Node
    {        
        /// <summary>
        /// Уникальная метка-идентификатор
        /// </summary>
        public Guid Label { get; }

        /// <summary>
        /// Флаг наличия перехода по goto на эту строку кода
        /// </summary>
        public bool IsLabeled { get; set; }

        public TA_Node() { Label = Guid.NewGuid(); }

        public override bool Equals(object obj)
        {
            if (obj is TA_Node)
                return Label == (obj as TA_Node).Label;
            return false;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }
    }

    /// <summary>
    /// Пустой оператор
    /// </summary>
    public class TA_Empty : TA_Node
    {
        public override string ToString()
        {
            return string.Format("{0} : nop", Label);
        }
    }

    /// <summary>
    /// Оператор присваивания
    /// </summary>
    public class TA_Assign : TA_Node
    {
        /// <summary>
        /// Левый операнд (null, если операция унарная)
        /// </summary>
        public TA_Expr Left { get; set; }

        /// <summary>
        /// Правый операнд
        /// </summary>
        public TA_Expr Right { get; set; }

        /// <summary>
        /// Хранилище результата
        /// </summary>
        public TA_Var Result { get; set; }

        /// <summary>
        /// Производимая операция
        /// </summary>
        public OpCode Operation { get; set; }

        public override string ToString()
        {
            if (Left == null)
                return string.Format("{0} : {1} = {2}{3}", Label, Result, Operation.GetSymbol(), Right);
            else
                return string.Format("{0} : {1} = {2} {3} {4}", Label, Result, Left, Operation.GetSymbol(), Right);
        }
    }

    /// <summary>
    /// Оператор безусловного перехода
    /// </summary>
    public class TA_Goto : TA_Node
    {
        /// <summary>
        /// Метка-идентификатор оператора, на который происходит переход
        /// </summary>
        public Guid TargetLabel { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : goto {1}", Label, TargetLabel);
        }
    }


    /// <summary>
    /// Оператор условного перехода
    /// </summary>
    public class TA_IfGoto : TA_Goto
    {
        /// <summary>
        /// Условие перехода
        /// </summary>
        public TA_Expr Condition { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : if {1} goto {2}", Label, Condition, TargetLabel);
        }
    }

    /// <summary>
    /// Оператор печати
    /// </summary>
    public class TA_Print : TA_Node
    {
        public TA_Expr Data { get; set; }
        public string Sep { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : print {1} \"{2}\"", Label, Data, Sep);
        }
    }


    /*
     * =========================================================
     * Операнды могут быть либо константами,
     * либо сгенерированными переменными
     * =========================================================
    */

    /// <summary>
    /// Базовый класс для операндов
    /// </summary>
    public class TA_Expr
    {
    }

    /// <summary>
    /// Операнд-константа (числа типа int)
    /// </summary>
    public class TA_IntConst : TA_Expr
    {
        public int Num { get; }

        public TA_IntConst(int num) { Num = num; }

        public override string ToString()
        {
            return Num.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is TA_IntConst)
                return Num == (obj as TA_IntConst).Num;
            return false;
        }

        public override int GetHashCode()
        {
            return Num.GetHashCode();
        }
    }

    /// <summary>
    /// Операнд-переменная
    /// </summary>
    public class TA_Var : TA_Expr
    {
        public Guid ID { get; }

        public TA_Var() { ID = Guid.NewGuid(); }

        public override string ToString()
        {
            return ID.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is TA_Var)
                return ID == (obj as TA_Var).ID;
            return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }

    

    /*
     * =========================================================
     * Код программы хранится в виде команд 
     * в формате трехадресного кода в списке
     * =========================================================
    */

    /// <summary>
    /// Класс для хранения программы в формате 
    /// трехадресного кода
    /// </summary>
    public class TACode
    {
        List<TA_Node> m_code;

        Dictionary<Guid, TA_Node> m_labeledCode;
        
        public IEnumerable<TA_Node> CodeList
        {
            get { return m_code; }                
        }

        public TACode()
        {
            m_code = new List<TA_Node>();
            m_labeledCode = new Dictionary<Guid, TA_Node>();
        }

        /// <summary>
        /// Добавить оператор
        /// </summary>
        public void AddNode(TA_Node node)
        {
            m_code.Add(node);
            m_labeledCode.Add(node.Label, node);
        }
        

        /// <summary>
        /// Удалить оператор
        /// </summary>
        /// <param name="node">Оператор</param>
        public bool RemoveNode(TA_Node node)
        {
            return m_code.Remove(node);
        }

        /// <summary>
        /// Удалить набор операторов
        /// </summary>
        /// <param name="nodes">Список</param>
        public void RemoveRange(IEnumerable<TA_Node> nodes)
        {
            foreach (var node in nodes)
                m_code.Remove(node);
        }        

        public override string ToString()
        {
            return m_code.Aggregate("", (s, node) => s + node.ToString() + Environment.NewLine);
        }
    }
}