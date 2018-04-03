// <copyright file="PrettyPrintTests.cs" company="mmcs">
//     Copyright (c) mmcs. All rights reserved.
// </copyright>
// <author>Alexander Svetly</author>
namespace Compiler.Parser.Tests
{
    using Compiler.Parser.Visitors;
    using NUnit.Framework;

    /// <summary>
    /// Тесты для PrettyPrintVisitor
    /// </summary>
    [TestFixture]
    public class PrettyPrintTests
    {
        /// <summary>
        /// Тесты для PrettyPrintVisitor
        /// </summary>
        [Test]
        public void TestCase()
        {
            var scanner = new Scanner();

            scanner.SetSource("a = 3;", 0);
            Assert.True(ParseScanner(scanner) == "  a = 3;\n");

            scanner.SetSource("while (0) b = 2;", 0);
            Assert.True(ParseScanner(scanner) == "  while(0)\n    b = 2;\n");

            scanner.SetSource("if (2){a = 1;} else a = 2;", 0);
            Assert.True(ParseScanner(scanner) == "  if (2)\n  {\n    a = 1;\n  }\n  else\n    a = 2;\n");

            scanner.SetSource("for (i = 0, 10){print(1 >= 3);if (1 + 3){a=1;}}", 0);
            Assert.True(ParseScanner(scanner) == "  for(i = 0,10,1)\n  {\n    print((1 >= 3));\n    if ((1 + 3))\n    {\n      a = 1;\n    }\n  }\n");
        }

        /// <summary>
        /// Создает Parser на основе Scanner и проходит по построенному дереву
        /// </summary>
        /// <param name="scanner">Scanner variable</param>
        /// <returns>
        /// Возвращает текст программы
        /// </returns>
        private static string ParseScanner(Scanner scanner)
        {
            var parser = new Parser(scanner);
            parser.Parse();

            var prettyPrintVisitor = new PrettyPrintVisitor();
            parser.root.Visit(prettyPrintVisitor);

            return prettyPrintVisitor.Text;
        }
    }
}
