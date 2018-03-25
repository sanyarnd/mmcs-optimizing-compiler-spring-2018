using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.Optimizations.Tests
{
	[TestFixture]
	public class ConstantFoldingTests
	{
		[Test]
		public void Test1()
		{
			var taCodeConstantFolding = new TACode();
			var assgn1 = new Assign()
			{
				Left = null,
				Operation = OpCode.Copy,
				Right = new Var(),
				Result = new Var()
			};
			var assgn2 = new Assign()
			{
				Left = new IntConst(20),
				Operation = OpCode.Mul,
				Right = new IntConst(3),
				Result = new Var()
			};
			var assgn3 = new Assign()
			{
				Left = new IntConst(10),
				Operation = OpCode.Plus,
				Right = new IntConst(1),
				Result = new Var()
			};
			var assgn4 = new Assign()
			{
				Left = new IntConst(100),
				Operation = OpCode.Div,
				Right = new IntConst(50),
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

			taCodeConstantFolding.AddNode(assgn1);
			taCodeConstantFolding.AddNode(assgn2);
			taCodeConstantFolding.AddNode(assgn3);
			taCodeConstantFolding.AddNode(assgn4);
			taCodeConstantFolding.AddNode(assgn5);
			taCodeConstantFolding.AddNode(assgn6);

			/*
			  a = b
			  c = 20 * 3    -----> c = 60
			  d = 10 + 1    -----> d = 11
			  e = 100 / 50  -----> e = 2
			  a = 30 - 20   -----> a = 10
			  k = c + a
			*/

			// TODO Assert'ы

			Assert.True(true);
		}
	}
}