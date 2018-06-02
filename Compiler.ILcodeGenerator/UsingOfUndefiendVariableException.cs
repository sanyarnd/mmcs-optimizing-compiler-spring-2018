using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ILcodeGenerator
{
    class UsingOfUndefiendVariableException : Exception
    {
        public UsingOfUndefiendVariableException(string message)
            : base(message)
        { }
    }
}
