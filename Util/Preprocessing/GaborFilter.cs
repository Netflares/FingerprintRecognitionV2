using System.Collections;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class GaborFilter
    {
        static readonly int 
            Height = ProcImg.Height, Width = ProcImg.Width, ImgSize = ProcImg.ImgSize,
            BS = ProcImg.BlockSize;

        static public void Apply(double[,] norm, double[,] orient, double waveLen, bool[,] msk)
        {
            Image<Gray, double> kernel = CreateBaseFilter(waveLen);

        }

        /** 
         * @ the base kernel for gabor filter
         * */
        static private Image<Gray, double> CreateBaseFilter(double wave)
        {
            double sigma = wave * 0.65;
            int kr = (int) Round(3 * sigma);    // kernel radius
            int ks = kr << 1 | 1;               // kernel size

            double[,] mx = new double[ks, ks];
            double[,] my = new double[ks, ks];

            Iterator2D.Forward(ks, ks, (y, x) =>
            {
                mx[y, x] = x - kr;
                my[y, x] = y - kr;
            });

            // gabor filter kernel equation:
            //      refa = - ( mx/(sigma**2) + my/(sigma**2) )
            //      refb = 2 * PI * waveLen * mx
            //      kernel = exp(refa) * cos(refb)
            Image<Gray, double> kernel = new(ks, ks);
            double sig2 = sigma * sigma, wpi = wave * PI;   // store what can be store (I don't trust O3)
            Iterator2D.Forward(ks, ks, (y, x) =>
            {
                double v = wpi * mx[y, x];
                kernel[y, x] = new Gray(
                    Exp(- mx[y, x] / sig2 - my[y, x] / sig2) * Cos(v + v)
                );
            });

            return kernel;
        }
    }
}
