using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Compiler.IDE
{
    public class TextBoxConsole : TextWriter
    {
        TextBox output = null;

        public TextBoxConsole(TextBox _output)
        {
            output = _output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            output.AppendText(value.ToString());
        }


        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
