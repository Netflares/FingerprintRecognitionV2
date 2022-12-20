using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // `ProcImg`: The Image after pre-processing
    public class ProcImg
    {
        /** 
         * @ public attrs
         * */

        /** 
         * @ shared attrs
         * 
         * to optimize memory resources
         * */
        static readonly int Height = 480, Width = 320;

        static double[,] NormMat = new double[Height, Width];

        /** 
         * @ pipline
         * */
        public ProcImg(Image<Gray, byte> src)
        {
            Stopwatch timer = new();
            timer.Start();

            NormMat = Normalization.Normalize(src, 100, 100);
            PrintTime(timer, "normalize");
        }

        /** 
         * @ debug
         * */
        static private void PrintTime(Stopwatch timer, string m)
        {
            Console.WriteLine(string.Format(
                "{0}: {1}.{2}", m, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds
            ));
        }
    }
}
