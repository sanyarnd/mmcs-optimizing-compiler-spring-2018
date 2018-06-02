using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ThreeAddrCode
{
    public class TACodeNameManager
    {
        public readonly static TACodeNameManager Instance = new TACodeNameManager();

        private Dictionary<Guid, String> _names = new Dictionary<Guid, string>();

        private uint _tempVarCounter = 0;
        private uint _blockCounter = 0;
        private uint _labelCounter = 0;

        private void AssignIfNotExists(Guid id, String name)
        {
            if (_names.ContainsKey(id)) return;
            _names[id] = name;
        }

        public void Name(Guid id, string name) => _names[id] = name;

        public void TempVar(Guid id) => AssignIfNotExists(id, $"t{_tempVarCounter++}");

        public void Block(Guid id) => AssignIfNotExists(id, $"b{_blockCounter++}");

        public void Label(Guid id) => AssignIfNotExists(id, $"l{_labelCounter++}");

        public string this[Guid id] => _names.ContainsKey(id) ? _names[id] : id.ToString();
    }
}
