using System;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace Compiler.IDE
{
    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread, HandleProcessCorruptedStateExceptions]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            try
            {
                Application.Run(mainWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(mainWindow, $@"Произошло что-то ужасное и программа вынуждена прервать выполение:{Environment.NewLine} {ex.Message}",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}