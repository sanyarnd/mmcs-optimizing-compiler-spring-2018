using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.Optimizations.Tests
{
    [TestFixture]
    public class CopyPropagationTests
    {
        [Test]
        public void Test1()
        {
            var taCodeCopyProp = new TACode();
            var assgn1 = new Assign()
            {
                Left = null,
                Operation = OpCode.Copy,
                Right = new IntConst(5),
                Result = new Var()
            };
            var assgn2 = new Assign()
            {
                Left = assgn1.Right,
                Operation = OpCode.Minus,
                Right = assgn1.Result,
                Result = new Var()
            };
            var assgn3 = new Assign()
            {
                Left = assgn2.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(1),
                Result = new Var()
            };
            var assgn4 = new Assign()
            {
                Left = assgn3.Result,
                Operation = OpCode.Mul,
                Right = assgn1.Result,
                Result = new Var()
            };
            var assgn5 = new Assign()
            {
                Left = new IntConst(30),
                Operation = OpCode.Minus,
                Right = new IntConst(20),
                Result = assgn1.Result
            };
            var assgn6 = new Assign()
            {
                Left = assgn2.Result,
                Operation = OpCode.Plus,
                Right = assgn5.Result,
                Result = new Var()
            };

            taCodeCopyProp.AddNode(assgn1);
            taCodeCopyProp.AddNode(assgn2);
            taCodeCopyProp.AddNode(assgn3);
            taCodeCopyProp.AddNode(assgn4);
            taCodeCopyProp.AddNode(assgn5);
            taCodeCopyProp.AddNode(assgn6);

            /*
              a = b
              c = b - a     -----> c = b - b
              d = c + 1
              e = d * a     -----> e = d * b
              a = 30 - 20
              k = c + a     -----> k = c + a
            */

            // TODO Assert'ы
            
            Assert.True(true);
        }
    }
}