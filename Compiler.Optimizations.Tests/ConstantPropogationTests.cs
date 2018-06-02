using System.Linq;
using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.Optimizations.Tests
{
	[TestFixture]
	public class ConstantPropogationTests
	{
		[Test]
		public void Test1()
		{
			var taCodeConstProp = new TACode();
			var assgn1 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new IntConst(10),
				Result = new Var()
			};
			var assgn2 = new Assign()
			{
				Left = new Var(),
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

			taCodeConstProp.AddNode(assgn1);
			taCodeConstProp.AddNode(assgn2);
			taCodeConstProp.AddNode(assgn3);
			taCodeConstProp.AddNode(assgn4);
			taCodeConstProp.AddNode(assgn5);
			taCodeConstProp.AddNode(assgn6);

			var optConstProp = new CopyPropagation();
			optConstProp.Optimize(taCodeConstProp.CodeList.ToList(), out var applCopProp);
			/*
              a = 10
              c = b - a     -----> c = b - 10
              d = c + 1
              e = d * a     -----> e = d * 10
              a = 30 - 20
              k = c + a     -----> k = c + a
            */

			Assert.AreEqual(assgn2.Right, assgn1.Result);
			Assert.AreEqual(assgn4.Right, assgn1.Result);
			Assert.AreNotSame(assgn6.Right, assgn1.Result);
			Assert.True(true);
		}
	}
}
