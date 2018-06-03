using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ThreeAddrCode.Nodes;
using Compiler.ThreeAddrCode.Expressions;

namespace Compiler.ThreeAddrCode.DFA.ConstantPropagation
{
	public class Operations : ILatticeOperations<Dictionary<Guid, VarValue>>
	{
		private Dictionary<Guid, VarValue> upper;
		private Dictionary<Guid, VarValue> lower;

		public Operations(TACode code)
		{
			upper = new Dictionary<Guid, VarValue>();
			foreach (var node in code.CodeList)
			{
				if (node is Assign)
				{
					var asNode = node as Assign;
					// т.к. node as Assign всегда Var верхние значения инициализированы как NAC
					if (!upper.ContainsKey((asNode.Result as Var).Id))
						upper.Add((asNode.Result as Var).Id, new VarValue(asNode.Result));
					if (asNode.Left is Var && !upper.ContainsKey((asNode.Left as Var).Id))
						upper.Add((asNode.Left as Var).Id, new VarValue(asNode.Left as Var));
					if (asNode.Right is Var && !upper.ContainsKey((asNode.Right as Var).Id))
						upper.Add((asNode.Right as Var).Id, new VarValue(asNode.Right as Var));
				}
			}
			lower = new Dictionary<Guid, VarValue>();
			foreach (var node in code.CodeList)
			{
				if (node is Assign)
				{
					var asNode = node as Assign;
					// UNDEF
					if (!lower.ContainsKey((asNode.Result as Var).Id))
						lower.Add((asNode.Result as Var).Id, new VarValue());
					if (asNode.Left is Var && !lower.ContainsKey((asNode.Left as Var).Id))
						lower.Add((asNode.Left as Var).Id, new VarValue());
					if (asNode.Right is Var && !lower.ContainsKey((asNode.Right as Var).Id))
						lower.Add((asNode.Right as Var).Id, new VarValue());
				}
			}
		}

		public Dictionary<Guid, VarValue> Lower => lower;

		public Dictionary<Guid, VarValue> Upper => upper;

		public bool? Compare(Dictionary<Guid, VarValue> a, Dictionary<Guid, VarValue> b)
		{
			if (a.Count != b.Count)
				return null;
			if (a.Keys.Except(b.Keys).Any())
				return null;
			if (b.Keys.Except(a.Keys).Any())
				return null;
			int countLess = 0, countMore = 0;
			foreach (var key in a.Keys)
			{
				if (a[key] <= b[key])
				{
					countLess++;
				}
				else
				{
					countMore++;
				}
			}
			if (countLess == 0 || countMore == 0)
				return countLess > countMore;
			return null;
		}

		public Dictionary<Guid, VarValue> Operator(Dictionary<Guid, VarValue> a, Dictionary<Guid, VarValue> b)
		{
			var aCopy = a.ToDictionary(entry => entry.Key, entry => entry.Value);
			var bCopy = b.ToDictionary(entry => entry.Key, entry => entry.Value);
			Dictionary<Guid, VarValue> result = new Dictionary<Guid, VarValue>();
			foreach (var key in aCopy.Keys)
			{
				result[key] = a[key].CollectionOperator(b[key]);
				if (bCopy.ContainsKey(key))
					bCopy.Remove(key);
			}
			foreach (var key in bCopy.Keys)
				result.Add(key, bCopy[key]);
			return result;
		}
	}
}
