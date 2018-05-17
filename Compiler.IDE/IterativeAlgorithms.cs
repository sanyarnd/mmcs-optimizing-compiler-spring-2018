using System;

namespace Compiler.IDE
{
    internal enum IterativeAlgorithms
    {
        ReachingDefs = 0,
        ReachingExprs = 1
    }

    internal static class IterativeAlgorithmsExt
    {
        public static string GetString(this IterativeAlgorithms opt)
        {
            switch (opt)
            {
                case IterativeAlgorithms.ReachingDefs:
                    return "Достигаемые определения";
                case IterativeAlgorithms.ReachingExprs:
                    return "Достигаемые выражения";
                default:
                    throw new NotImplementedException($"key {opt} is missing");
            }
        }
    }
}