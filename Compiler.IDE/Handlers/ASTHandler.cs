using System;
using System.Drawing;
using System.IO;
using System.Text;
using Compiler.Parser.AST;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Compiler.IDE.Handlers
{
    internal class AstHandler
    {
        public event EventHandler<Image> GenerationCompleted = delegate {};

        public void GenerateAstImage(BlockNode root)
        {
            string graph = BuildDotGraph(root);
            File.WriteAllText(@"ast_graph.txt", graph);

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
        }

        private static string BuildDotGraph(Node root)
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph AST {");
            sb.AppendLine("node[shape = record]\n");
            sb.AppendLine("graph [splines=ortho, nodesep=1, overlap=false];");
            var nodeVisitor = new AstGraphvizVisitor();
            root.Visit(nodeVisitor);
            sb.AppendLine(nodeVisitor.Nodes);
            sb.AppendLine("\n");
            sb.AppendLine(nodeVisitor.Edges);
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
