using System.Linq;
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
				Right = new Var(),
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

			var optCopyProp = new CopyPropagation();
			optCopyProp.Optimize(taCodeCopyProp.CodeList.ToList(), out var applCopProp);
			/*
              a = b
              c = b - a     -----> c = b - b
              d = c + 1
              e = d * a     -----> e = d * b
              a = 30 - 20
              k = c + a     -----> k = c + a
            */

			Assert.AreEqual(assgn2.Right, assgn1.Right);
			Assert.AreEqual(assgn4.Right, assgn1.Right);
			Assert.AreNotSame(assgn6.Right, assgn1.Right);
			Assert.True(true);
        }
    }
}