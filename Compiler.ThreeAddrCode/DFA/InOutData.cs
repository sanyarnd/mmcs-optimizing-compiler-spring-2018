using System;
using System.Collections.Generic;
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
    }
}