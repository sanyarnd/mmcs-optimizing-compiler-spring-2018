using System;
using System.Collections.Generic;
using System.Text;
using Compiler.ThreeAddrCode.CFG;

namespace Compiler.ThreeAddrCode.DFA
{
    /// <summary>
    ///     Класс-обертка над Generic.Dictionary для возможности
    ///     расширения функционала и упрошения синтаксиса
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InOutData<T> : Dictionary<BasicBlock, (T, T)>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("++++");
            foreach(var kv in this)
            {
                sb.AppendLine(kv.Key + ":\n" + kv.Value);
            }
            sb.AppendLine("++++");
            return sb.ToString();
        }
    }
}