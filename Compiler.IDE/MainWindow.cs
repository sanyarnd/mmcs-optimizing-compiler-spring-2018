using Compiler.IDE.Handlers;
using Compiler.ILcodeGenerator;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IterativeAlgorithmHandler _algoHandler = new IterativeAlgorithmHandler();
        private readonly AstHandler _astHandler = new AstHandler();

        private readonly IlCodeHandler _ilCodeHandler = new IlCodeHandler();
        private TAcode2ILcodeTranslator _ilProgram;
        private Thread _runThread;

        public MainWindow()
        {
            InitializeComponent();

            InitOptimizations();
            InitIterativeAlgorithms();

            InitOutputListeners();
            InitCommonListeners();
            InitInputTabListeners();
            InitCfgTabListeners();
            InitAstTabListeners();
        }

        private void InitOptimizations()
        {
            // populate listbox with enums
            foreach (Optimizations opt in Enum.GetValues(
                typeof(Optimizations)))
                optsList.Items.Add(opt);

            // enable custom formatting for listbox
            optsList.FormattingEnabled = true;
            optsList.Format += (s, e) => { e.Value = $"{((Optimizations) e.ListItem).GetString()}"; };

            // on item click enable/disable optimization in three addr code hadler
            optsList.ItemCheck += (o, e) =>
            {
                var opt = (Optimizations) optsList.Items[e.Index];
                _threeCodeHandler.OptimizationList[opt] = e.NewValue == CheckState.Checked;
            };
        }

        private void InitIterativeAlgorithms()
        {
            // populate listbox with enums
            foreach (IterativeAlgorithms opt in Enum.GetValues(
                typeof(IterativeAlgorithms)))
                iterativeAlgoList.Items.Add(opt);

            // enable custom formatting
            iterativeAlgoList.FormattingEnabled = true;
            iterativeAlgoList.Format += (s, e) => { e.Value = $"{((IterativeAlgorithms) e.ListItem).GetString()}"; };

            // on item click enable/disable algorithm
            iterativeAlgoList.ItemCheck += (o, e) =>
            {
                // select algorithm from list
                var algo = (IterativeAlgorithms) iterativeAlgoList.Items[e.Index];

                // pass algorithm to handler
                _algoHandler.IterativeAlgoList[algo] = e.NewValue == CheckState.Checked;
            };
        }

        private void InitCommonListeners()
        {
            // open/exit
            exitToolStripMenuItem.Click += (o, e) => Application.Exit();
            openToolStripMenuItem.Click += (o, e) => OpenSourceFile();

            // compile button
            compileButton.Click += (o, e) => ClearOutput();
            compileButton.Click += (o, e) => _parseHandler.Parse(inputTextBox.Text);

            // toggle opts
            toggleOptsButton.Click += (o, e) =>
            {
                bool allChecked = true;
                for (int i = 0; i < optsList.Items.Count; ++i)
                    allChecked &= optsList.GetItemChecked(i);

                for (int i = 0; i < optsList.Items.Count; ++i)
                    optsList.SetItemChecked(i, !allChecked);
            };
            optsList.SelectedIndexChanged += (o, e) => optsList.ClearSelected();
            iterativeAlgoList.SelectedIndexChanged += (o, e) => iterativeAlgoList.ClearSelected();

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

            // iterative algorithms
            _cfgHandler.CfgGenerated += (o, e) => _algoHandler.CollectInOutData(e);
            _algoHandler.PrintableInOutDataCollected += (o, e) => iterativeAlgoTextBox.Text = e;

            // IL-code
            _threeCodeHandler.GenerationCompleted += (o, e) =>
            {
                try
                {
                    _ilCodeHandler.GenerateIlCode(e);
                    runButton.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $@"Компиляция завершилась с ошибкой:{Environment.NewLine} {ex.Message}",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _ilCodeHandler.GenerationCompleted += (o, e) =>
            {
                ILCodeTextBox.Text = e.PrintCommands();
                _ilProgram = e;
            };

            // run and stop
            // no way to pass cancellation token inside DynMethod's Invoke, so doing it hard way =\
            stopButton.Click += (o, e) => _runThread?.Abort();
            runButton.Click += (o, e) =>
            {
                if (_runThread != null)
                {
                    MessageBox.Show(this, @"Программа уже запущена!", @"Ошибка", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ClearOutput();
                var runTask = Task.Factory.StartNew(() =>
                {
                    _runThread = Thread.CurrentThread;
                    try
                    {
                        _ilProgram.RunProgram();
                    }
                    catch (ThreadAbortException)
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                MessageBox.Show(this, @"Запуск остановлен", @"Остановка", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                outTextBox.AppendText($"{Environment.NewLine}ОСТАНОВКА");
                            }));
                            _runThread = null;
                        }
                        else
                        {
                            MessageBox.Show(this, @"Запуск остановлен", @"Остановка", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $@"Запуск завершился с ошибкой:{Environment.NewLine} {ex.Message}",
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            };

            // enable UI after all build steps
            _cfgHandler.GenerationCompleted += (o, e) =>
            {
                cfgSaveButton.Enabled = true;
                cfgScaleBar.Enabled = true;

                astSaveButton.Enabled = true;
                astTrackBar.Enabled = true;
            };
        }

        private void InitOutputListeners()
        {
            // redirect console to textbox
            Console.SetOut(new TextBoxConsole(outTextBox));

            _parseHandler.ParsingErrored += (o, e) =>
                outTextBox.AppendText("Ошибка парсинга программы" + Environment.NewLine);
            _parseHandler.ParsingSyntaxErrored += (o, e) =>
                outTextBox.AppendText($"Синтаксическая ошибка. {e.Message}" + Environment.NewLine);
            _parseHandler.ParsingLexErrored += (o, e) =>
                outTextBox.AppendText($"Лексическая ошибка. {e.Message}" + Environment.NewLine);

            _parseHandler.ParsingCompleted += (o, e) =>
                outTextBox.AppendText("Синтаксическое дерево построено" + Environment.NewLine);
            _threeCodeHandler.PrintableCodeGenerated += (o, e) =>
                outTextBox.AppendText("Создание трехадресного кода завершено" + Environment.NewLine);
            _cfgHandler.GenerationCompleted += (o, e) =>
                outTextBox.AppendText("Граф потока управления построен" + Environment.NewLine);
            _astHandler.GenerationCompleted +=
                (o, e) => outTextBox.AppendText("Граф AST построен" + Environment.NewLine);
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
            var format = ImageFormat.Png;
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