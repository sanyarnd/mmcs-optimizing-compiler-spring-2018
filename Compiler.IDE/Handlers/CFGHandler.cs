using System;
using System.Drawing;
using System.IO;
using System.Text;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.CFG;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Compiler.IDE.Handlers
{
    internal class CfgHandler
    {
        public event EventHandler<Image> GenerationCompleted = delegate { };
        public event EventHandler<ControlFlowGraph> CfgGenerated = delegate { };
        public event EventHandler<Exception> GenerationErrored = delegate { };


        public void GenerateCFG(TACode code)
        {
            try
            {
                var cfg = new ControlFlowGraph(code);

                string graph = BuildDotGraph(cfg);
                File.WriteAllText(@"cfg_graph.txt", graph);

                var processQuery = new GetStartProcessQuery();
                var processStartInfoQuery = new GetProcessStartInfoQuery();
                var registerLayout = new RegisterLayoutPluginCommand(processStartInfoQuery, processQuery);
                var wrapper = new GraphGeneration(processQuery, processStartInfoQuery, registerLayout)
                {
                    RenderingEngine = Enums.RenderingEngine.Dot
                };
                byte[] output = wrapper.GenerateGraph(graph, Enums.GraphReturnType.Png);

                using (var stream = new MemoryStream(output))
                {
                    var image = Image.FromStream(stream);
                    GenerationCompleted(null, image);
                }

                CfgGenerated(null, cfg);
            }
            catch (Exception ex)
            {
                GenerationErrored(null, ex);
            }
        }

        private static string GetNameForGuid(Guid guid)
        {
            // добавляем какой-нибудь текстовый префикс
            return "b" + guid.ToString().Substring(0, 5);
        }

        private static string BuildDotGraph(ControlFlowGraph cfg)
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph CFG {");
            sb.AppendLine("node[shape = record];\n");
            sb.AppendLine("graph [splines=ortho, nodesep=1, overlap=false];");
            foreach (var node in cfg.CFGNodes)
            {
                string name = GetNameForGuid(node.BlockId);

                // <ИМЯ_УЗЛА> [label = "<TEXT>\l<TEXT>"]
                sb.Append($"{name}  [label = \"");
                foreach (var cmd in node.CodeList)
                {
                    string cmdText = OutputSanitizer.Sanitize(cmd.ToString(), OutputSanitizer.SanitizeLevel.DotFile);
                    sb.Append(cmdText + @"\l");
                }

                sb.AppendLine("\"]");
            }

            sb.AppendLine("\n");

            // генерируем дуги
            foreach (var node in cfg.CFGNodes)
            {
                string name = GetNameForGuid(node.BlockId);
                // строим по потомкам
                foreach (var c in node.Children)
                {
                    string cName = GetNameForGuid(c.BlockId);
                    sb.AppendLine($"{name} -> {cName};");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}