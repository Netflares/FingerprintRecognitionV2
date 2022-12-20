using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Normalization
    {
        /** 
         * @ First Normalization stage
         * */
        static public double[,] Normalize(Image<Gray, byte> src, double m0, double v0)
        {
            /*
                `src`:
                    the source image
                    white fg, black bg
                `res`:
                    the img after normalization
                    black fg, white bg
                `m0`:
                    avg value of `res`
                `v0`:
                    color span of `res`
            */
            double[,] res = new double[src.Height, src.Width];

            double avg = src.GetAverage().Intensity;
            double std = Iterator2D.Sum(src, (y, x) => Sqr(src[y, x].Intensity - avg));
            std = Sqrt(std / (src.Height * src.Width));

            double v0sqrt = Sqrt(v0);

            Iterator2D.Forward(
                res, (y, x) => res[y, x] = NormalizePixel(m0, v0sqrt, src[y, x].Intensity, avg, std)
            );

            return res;
        }

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;

        static private double NormalizePixel(double m0, double v0sqrt, double px, double avg, double std)
        {
            double coeff = Abs(px - avg) * v0sqrt / std;
            // flip fg/bg color here
            if (px < std)
                return m0 + coeff;
            return m0 - coeff;
        }
    }
}
