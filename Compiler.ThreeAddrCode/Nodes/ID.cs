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

    public class PrettyGuid
    {
        Guid id;
        string title;

        //Guid ID => id;

        public PrettyGuid(IDType type, string name = "")
        {
            id = Guid.NewGuid();
            if (name == "")
            {
                switch (type)
                {
                    case IDType.Label:
                        title = TitleGen.GenLabelTitle();
                        break;

                    case IDType.Var:
                        title = TitleGen.GenVarTitle();
                        break;

                    default:
                        title = "Unknown";
                        break;
                }
            }
            else
                title = name;
        }

        public static implicit operator Guid(PrettyGuid pretty)
        {
            return pretty.id;
        }

        public override bool Equals(object obj)
        {
            if (obj is PrettyGuid idinfo)
                return id.Equals(idinfo.id);
            return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(PrettyGuid left, PrettyGuid right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PrettyGuid left, PrettyGuid right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return title;
        }
    }

    class TitleGen
    {
        static int Lcount, Vcount;


        public static string GenLabelTitle() => "Label_" + (Lcount++).ToString("0000");


        public static string GenVarTitle() => "Var_" + (Vcount++).ToString("0000");

    }
}
