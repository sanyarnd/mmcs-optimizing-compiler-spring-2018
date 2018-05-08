using Compiler.Parser.AST;
using Compiler.Parser.Visitors;
using System;
using Compiler.ThreeAddrCode;

namespace Compiler.IDE.Handlers
{
    internal class ThreeAddrCodeHandler
    {
        public event EventHandler<ThreeAddrCode.TACode> GenerationCompleted = delegate {};
        public event EventHandler<string> PrintableCodeGenerated = delegate {};

        public void GenerateThreeAddrCode(object sender, BlockNode e)
        {
            var visitor = new TACodeVisitor();
            e.Visit(visitor);

            GenerationCompleted(null, visitor.Code);

            PostProcess(visitor.Code);
        }

        private void PostProcess(TACode code)
        {
            string postProcessCode = OutputSanitizer.Sanitize(code.ToString(), OutputSanitizer.SanitizeLevel.TextBox);
            PrintableCodeGenerated(null, postProcessCode);
        }
    }
}
