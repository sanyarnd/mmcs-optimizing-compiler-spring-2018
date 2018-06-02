using System;

namespace Compiler.IDE
{
    internal static class OutputSanitizer
    {
        public enum SanitizeLevel
        {
            TextBox,
            DotFile
        };

        /// <summary>
        ///     Заменить недоспустимые последовательности на аналоги, например \n, " и т.п.
        /// </summary>
        /// <param name="input">данные для обработки</param>
        /// <param name="level">уровень обработки</param>
        /// <returns>обработанные данные</returns>
        public static string Sanitize(string input, SanitizeLevel level)
        {
            switch (level)
            {
                case SanitizeLevel.TextBox:
                    return input.Replace($"\"{Environment.NewLine}\"", "CLRF");
                case SanitizeLevel.DotFile:
                    return input.Replace($"\"{Environment.NewLine}\"", "CLRF")
                        .Replace("<", @"\<")
                        .Replace(">", @"\>")
                        .Replace("\"", "'");
                default:
                    return input;
            }
        }
    }
}