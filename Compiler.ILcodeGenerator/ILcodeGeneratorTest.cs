using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.Visitors;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ILcodeGenerator
{

    [TestFixture]
    class ILcodeGeneratorTest
    {
        [Test]
        public void ILcodeGeneratorTest1()
        {
            string fileName = @"sample.txt";
            //string temp=System.IO.Directory.GetCurrentDirectory();
            var astRoot = AST(fileName);
            if (astRoot == null)
                return;

            var tacodeVisitor = new TACodeVisitor();
            astRoot.Visit(tacodeVisitor);
            var tacodeInstance = tacodeVisitor.Code;
            string tastring=tacodeInstance.ToString();
            TAcode2ILcodeTranslator trans = new TAcode2ILcodeTranslator();
            trans.Translate(tacodeInstance);
            var temp=trans.PrintCommands();
            trans.RunProgram();
        }




        private static BlockNode AST(string fileName)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                string text = File.ReadAllText(fileName);

                var scanner = new Scanner();
                scanner.SetSource(text, 0);

                var parser = new Parser.Parser(scanner);

                var b = parser.Parse();
                if (b)
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    return parser.root;
                }
                else
                {
                    Console.WriteLine("Ошибка");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл {fileName} не найден");
            }
            catch (LexException e)
            {
                Console.WriteLine($"Лексическая ошибка. {e.Message}");
            }
            catch (SyntaxException e)
            {
                Console.WriteLine($"Синтаксическая ошибка. {e.Message}");
            }
            return null;
        }
    }
}
