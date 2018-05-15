using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ThreeAddrCode.Nodes
{
    public enum IDType
    {
        Label,
        Var
    }
    
    class TitleGen
    {
        static int Lcount, Vcount;


        public static string GenLabelTitle() => "Label_" + (Lcount++).ToString("0000");


        public static string GenVarTitle() => "Var_" + (Vcount++).ToString("0000");

    }
}
