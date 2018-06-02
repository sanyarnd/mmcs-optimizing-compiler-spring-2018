using System.Drawing;
using System.Windows.Forms;

namespace Compiler.IDE
{
    internal static class Utils
    {
        private const double ScaleMin = 0.1;
        private const double ScaleMax = 1;

        public static double TrackBarToScale(TrackBar bar)
        {
            return MapToRange(bar.Value, bar.Minimum, bar.Maximum, ScaleMin, ScaleMax);
        }

        public static double MapToRange(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static Image ScaleImage(Image original, double scaleValue)
        {
            var size = new Size((int) (original.Width * scaleValue), (int) (original.Height * scaleValue));
            return new Bitmap(original, size);
        }
    }
}