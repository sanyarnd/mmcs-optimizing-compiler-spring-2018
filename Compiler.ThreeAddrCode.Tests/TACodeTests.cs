using Compiler.ThreeAddrCode.Expressions;
using Compiler.ThreeAddrCode.Nodes;
using NUnit.Framework;

namespace Compiler.ThreeAddrCode.Tests
{
    [TestFixture]
    public class TaCodeTests
    {
        [Test]
        public void Test1()
        {
            var taCode = new TACode();

            var ass1 = new Assign
            {
                Left = new IntConst(3),
                Operation = OpCode.Minus,
                Right = new IntConst(5),
                Result = new Var()
            };

            var ass2 = new Assign
            {
                Left = new IntConst(10),
                Operation = OpCode.Plus,
                Right = new IntConst(2),
                Result = new Var()
            };

            var ass3 = new Assign
            {
                Operation = OpCode.Minus,
                Right = new IntConst(1),
                Result = new Var()
            };

            var ifgt1 = new IfGoto
            {
                Condition = new IntConst(1),
                TargetLabel = ass3.Label
            };
            ass3.IsLabeled = true;

            var ass4 = new Assign
            {
                Left = ass3.Result,
                Operation = OpCode.Plus,
                Right = new IntConst(1999),
                Result = new Var()
            };

            var ifgt2 = new IfGoto
            {
                Condition = new IntConst(2),
                TargetLabel = ass2.Label
            };
            ass2.IsLabeled = true;

            var ass5 = new Assign
            {
                Left = new IntConst(7),
                Operation = OpCode.Mul,
                Right = new IntConst(4),
                Result = new Var()
            };

            var ass6 = new Assign
            {
                Left = new IntConst(100),
                Operation = OpCode.Div,
                Right = new IntConst(25),
                Result = new Var()
            };

            taCode.AddNode(ass1);
            taCode.AddNode(ass2);
            taCode.AddNode(ass3);
            taCode.AddNode(ifgt1);
            taCode.AddNode(ass4);
            taCode.AddNode(ifgt2);
            taCode.AddNode(ass5);
            taCode.AddNode(ass6);
            
            // TODO Assert'ы
        }
		public void TestCopyPropagation()
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
		}
	}
}