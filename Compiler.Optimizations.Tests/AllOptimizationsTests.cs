using Compiler.ThreeAddrCode;
using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.Optimizations.Tests
{
	[TestFixture]
	public class AllOptimizationsTests
	{
		[Test]
		public void Test1()
		{
			var taCodeAllOptimizations = new TACode();
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
				Left = null,
				Operation = OpCode.Copy,
				Right = new IntConst(20),
				Result = new Var()
			};
			var assgn4 = new Assign()
			{
				Left = new IntConst(20),
				Operation = OpCode.Mul,
				Right = new IntConst(3),
				Result = new Var()
			};
			var assgn5 = new Assign()
			{
				Left = new IntConst(10),
				Operation = OpCode.Plus,
				Right = assgn3.Result,
				Result = new Var()
			};
			taCodeAllOptimizations.AddNode(assgn1);
			taCodeAllOptimizations.AddNode(assgn2);
			taCodeAllOptimizations.AddNode(assgn3);
			taCodeAllOptimizations.AddNode(assgn4);
			taCodeAllOptimizations.AddNode(assgn5);

			/*
			  a = b
			  c = b - a   -----> c = 0
			  n = 20
			  c = 20 * 3  -----> c = 60
			  d = 10 + n  -----> d = 30
			*/
			var allOptimizations = new AllOptimizations();
			allOptimizations.ApplyAllOptimizations(taCodeAllOptimizations);

			Assert.AreEqual(assgn2.Right, 0);
			Assert.AreEqual(assgn4.Right, 60);
			Assert.AreEqual(assgn5.Right, 30);
			Assert.True(true);
		}
	}
}
