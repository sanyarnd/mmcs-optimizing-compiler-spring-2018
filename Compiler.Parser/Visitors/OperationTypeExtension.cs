using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser.AST;

namespace Compiler.Parser.Visitors
{
    public static class OperationTypeExtensions
    {
        public static string ToSymbolString(this OperationType me)
        {
            switch (me)
            {
                case OperationType.Div: return "/";
                case OperationType.Equal: return "==";
                case OperationType.Greater: return ">";
                case OperationType.GreaterEq: return ">=";
                case OperationType.Less: return "<";
                case OperationType.LessEq: return "<=";
                case OperationType.Minus: return "-";
                case OperationType.Mul: return "*";
                case OperationType.Not: return "!";
                case OperationType.NotEqual: return "!=";
                case OperationType.Plus: return "+";
                case OperationType.UnaryMinus: return "-";
                default:
                    return "Bro it's forbidden operation!";
            }
        }
    }
}
