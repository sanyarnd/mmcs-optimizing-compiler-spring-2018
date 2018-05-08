using System;
using Compiler.ILcodeGenerator;
using Compiler.ThreeAddrCode;

namespace Compiler.IDE.Handlers
{
    internal class IlCodeHandler
    {
        public event EventHandler<TAcode2ILcodeTranslator> GenerationCompleted = delegate { };

        internal void GenerateILCode(TACode code)
        {
            TAcode2ILcodeTranslator trans = new TAcode2ILcodeTranslator();
            trans.Translate(code);
            GenerationCompleted(null, trans);
        }
    }
}