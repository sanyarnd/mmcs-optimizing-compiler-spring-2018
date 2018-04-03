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

            // a = 3;
            scanner.SetSource("a = 3;", 0);
            Assert.True(this.ParseScanner(scanner) == "  a = 3;\n");

            // while(0)
            //   b = 2;
            scanner.SetSource("while (0) b = 2;", 0);
            Assert.True(this.ParseScanner(scanner) == "  while(0)\n    b = 2;\n");

            // if (2)
            // {
            //   a = 1;
            // }
            // else
            //   a = 2;
            scanner.SetSource("if (2){a = 1;} else a = 2;", 0);
            Assert.True(this.ParseScanner(scanner) == "  if (2)\n  {\n    a = 1;\n  }\n  else\n    a = 2;\n");

            // for (i = 0, 10)
            // {
            //   print(1 >= 3);
            //   if (1 + 3)
            //   {
            //     a = 1;
            //   }
            // }
            scanner.SetSource("for (i = 0, 10){print(1 >= 3);if (1 + 3){a=1;}}", 0);
            Assert.True(this.ParseScanner(scanner) == "  for(i = 0,10,1)\n  {\n    print((1 >= 3));\n    if ((1 + 3))\n    {\n      a = 1;\n    }\n  }\n");
        }

        /// <summary>
        /// Создает Parser на основе Scanner и проходит по построенному дереву
        /// </summary>
        /// <param name="scanner">Scanner variable</param>
        /// <returns>
        /// Возвращает текст программы
        /// </returns>
        private string ParseScanner(Scanner scanner)
        {
            var parser = new Parser(scanner);
            parser.Parse();

            var prettyPrintVisitor = new PrettyPrintVisitor();
            parser.root.Visit(prettyPrintVisitor);

            return prettyPrintVisitor.Text;
        }
    }
}
