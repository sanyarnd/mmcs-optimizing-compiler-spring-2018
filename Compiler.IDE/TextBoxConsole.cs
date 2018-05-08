using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Compiler.IDE
{
    public class TextBoxConsole : TextWriter
    {
        private readonly TextBox _output;

        public TextBoxConsole(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString());
        }


        public override Encoding Encoding => Encoding.UTF8;
    }
}
