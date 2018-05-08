using Compiler.ThreeAddrCode.Nodes;
using System;

namespace Compiler.ThreeAddrCode.Expressions
{
    /// <summary>
    ///     Операнд-переменная
    /// </summary>
    public class Var : Expr
    {        
        public PrettyGuid Id { get; set; }
        
        public Var(string name = "")
        {
            Id = new PrettyGuid(IDType.Var, name);
        }

        //public Var(Guid id)
        //{
        //    Id = id;
        //}

        public override string ToString()
        {
            return Id.ToString();
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Id.Equals(((Var) obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Var left, Var right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Var left, Var right)
        {
            return !Equals(left, right);
        }
    }
}