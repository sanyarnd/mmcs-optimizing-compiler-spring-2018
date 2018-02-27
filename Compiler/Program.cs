using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.Visitors;
using System.IO;

namespace Compiler
{
    class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string FileName = @"..\..\sample.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                var parser = new Parser.Parser(scanner);

                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");

                    var autoVisitor = new AutoVisitor();
                    parser.root.Visit(autoVisitor);

                    var prettyPrinter = new PrettyPrintVisitor();
                    parser.root.Visit(prettyPrinter);
                    Console.WriteLine(prettyPrinter.Text);
                    //foreach (var st in parser.root.StList)
                    //Console.WriteLine(st);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Синтаксическая ошибка. " + e.Message);
            }

            Console.ReadLine();
        }
    }
}
