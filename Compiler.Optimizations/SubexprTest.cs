using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.CFG;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Optimizations
{
    public class SubexprTest
    {
        public void SubexpressionOptimizationTest()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new Var(),
                Operation = OpCode.Plus,
                Right = new Var(),
                Result = new Var()
            };

            var ass2 = new Assign
            {
                Left = ass1.Result,
                Operation = OpCode.Minus,
                Right = new Var(),
                Result = ass1.Left as Var
            };

            var ass3 = new Assign
            {
                Operation = OpCode.Plus,
                Left = ass2.Result,
                Right = ass1.Right,
                Result = ass1.Right as Var
            };
            var ass4 = new Assign
            {
                Left = ass1.Result,
                Operation = OpCode.Minus,
                Right = ass2.Right,
                Result = ass2.Right as Var
            };
            /*
            var ass7 = new Assign
            {
                Operation = OpCode.Mul,
                Left = ass3.Result,
                Right = new IntConst(3),
                Result = new Var()
            };*/

            /*
                a = b + c
                b = a - d
                c = b + c
                d = a - d  -----> d = b
            */

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ass4);

            Console.WriteLine("SUBEXPRESSION TEST");
            Console.WriteLine($"TA Code:\n{taCode.ToString()}");
            var subexpOpt = new SubexpressionOptimization();

            Console.WriteLine("Optimised TA Code:");
            foreach (var node in subexpOpt.Optimize(taCode.CodeList.ToList(), out var applied))
                Console.WriteLine($"{node}");
            Console.ReadKey();
        }
    }
}
