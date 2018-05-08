using Compiler.IDE.Handlers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Compiler.IDE
{
    public partial class MainWindow : Form
    {
        private readonly OpenFileDialog _sourceDialog = new OpenFileDialog();
        private readonly SaveFileDialog _saveGraphDialog = new SaveFileDialog();
        private Image _cfgImage;
        private Image _astImage;

        private readonly ParseHandler _parseHandler = new ParseHandler();
        private readonly ThreeAddrCodeHandler _threeCodeHandler = new ThreeAddrCodeHandler();
        private readonly CfgHandler _cfgHandler = new CfgHandler();
        private readonly AstHandler _astHandler = new AstHandler();

        public MainWindow()
        {
            InitializeComponent();
            
            InitCommonListeners();
            InitOutputListeners();
            InitInputTabListeners();
            InitCfgTabListeners();
            InitAstTabListeners();
        }

        private void InitCommonListeners()
        {
            // open/exit
            exitToolStripMenuItem.Click += (o, e) => Application.Exit();
            openToolStripMenuItem.Click += (o, e) => OpenSourceFile();

            // compile button
            compileButton.Click += (o, e) => ClearOutput();
            compileButton.Click += (o, e) => _parseHandler.Parse(inputTextBox.Text);

            // successful compilation
            _parseHandler.ParsingCompleted += (o, e) =>
            {
                runButton.Enabled = true;

                cfgSaveButton.Enabled = true;
                cfgScaleBar.Enabled = true;

                astSaveButton.Enabled = true;
                astTrackBar.Enabled = true;
            };

            // AST
            _parseHandler.ParsingCompleted += (o, e) => _astHandler.GenerateAstImage(e);
            _astHandler.GenerationCompleted += (o, e) =>
            {
                _astImage = e;
                ASTPictureBox.Image = _astImage;
                astTrackBar.Value = astTrackBar.Maximum;
            };

            // 3-addr code
            _parseHandler.ParsingCompleted += _threeCodeHandler.GenerateThreeAddrCode;
            _threeCodeHandler.PrintableCodeGenerated += (o, e) => threeAddrTextBox.Text = e;

            // CFG
            _threeCodeHandler.GenerationCompleted += (o, e) => _cfgHandler.GenerateCfgImage(e);
            _cfgHandler.GenerationCompleted += (o, e) =>
            {
                _cfgImage = e;
                CFGPictureBox.Image = _cfgImage;
                cfgScaleBar.Value = cfgScaleBar.Maximum;
            };
        }

        private void InitOutputListeners()
        {
            _parseHandler.ParsingErrored += (o, e) => outTextBox.AppendText("Ошибка парсинга программы" + Environment.NewLine);
            _parseHandler.ParsingSyntaxErrored += (o, e) => outTextBox.AppendText($"Синтаксическая ошибка. {e.Message}" + Environment.NewLine);
            _parseHandler.ParsingLexErrored += (o, e) => outTextBox.AppendText($"Лексическая ошибка. {e.Message}" + Environment.NewLine);

            _parseHandler.ParsingCompleted += (o, e) => outTextBox.AppendText("Синтаксическое дерево построено" + Environment.NewLine);
            _threeCodeHandler.PrintableCodeGenerated += (o, e) => outTextBox.AppendText("Создание трехадресного кода завершено" + Environment.NewLine);
            _cfgHandler.GenerationCompleted += (o, e) => outTextBox.AppendText("Граф потока управления построен" + Environment.NewLine);
            _astHandler.GenerationCompleted += (o, e) => outTextBox.AppendText("Граф AST построен" + Environment.NewLine);
        }

        private void InitInputTabListeners()
        {
            inputTextBox.TextChanged += (o, e) => runButton.Enabled = false;
        }

        private void InitCfgTabListeners()
        {
            cfgScaleBar.ValueChanged += (o, e) => 
                    CFGPictureBox.Image = Utils.ScaleImage(_cfgImage, Utils.TrackBarToScale(cfgScaleBar));
            cfgSaveButton.Click += (o, e) => SaveGraphFile(CFGPictureBox);
        }

        private void InitAstTabListeners()
        {
            astTrackBar.ValueChanged += (o, e) =>
                    ASTPictureBox.Image = Utils.ScaleImage(_astImage, Utils.TrackBarToScale(astTrackBar));
            astSaveButton.Click += (o, e) => SaveGraphFile(ASTPictureBox);
        }

        private void ClearOutput()
        {
            outTextBox.Text = "";
        }

        private void OpenSourceFile()
        {
            _sourceDialog.InitialDirectory = Directory.GetCurrentDirectory();
            _sourceDialog.Filter = @"txt files (*.txt)|*.txt|All files (*.*)|*.*";
            _sourceDialog.FilterIndex = 2;
            _sourceDialog.RestoreDirectory = true;

            if (_sourceDialog.ShowDialog() != DialogResult.OK)
                return;

            inputTextBox.Text = File.ReadAllText(_sourceDialog.FileName);
        }

        private void SaveGraphFile(PictureBox picbox)
        {
            _saveGraphDialog.Filter = @"Images|*.png;*.bmp;*.jpg";
            _saveGraphDialog.RestoreDirectory = true;
            ImageFormat format = ImageFormat.Png;
            if (_saveGraphDialog.ShowDialog() != DialogResult.OK)
                return;

            string ext = Path.GetExtension(_saveGraphDialog.FileName);
            switch (ext)
            {
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
            }
            picbox.Image.Save(_saveGraphDialog.FileName, format);
        }
    }
}
