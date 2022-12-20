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
        public bool[,] SegmentMsk;

        /** 
         * @ shared attrs
         * 
         * to optimize memory resources
         * */
        static readonly int Height = 480, Width = 320, BlockSize = 16;
        static readonly double AVG0 = 100;

        static public double[,] NormMat = new double[Height, Width];

        /** 
         * @ pipline
         * */
        public ProcImg(Image<Gray, byte> src)
        {
            Stopwatch timer = new();
            timer.Start();

            NormMat = Normalization.Normalize(src, AVG0, 100);
            PrintTime(timer, "normalize");

            SegmentMsk = Segmentation.CreateMask(NormMat, AVG0, BlockSize);
            PrintTime(timer, "segmentation");
        }

        /** 
         * @ debug
         * */
        static public void PrintTime(Stopwatch timer, string m)
        {
            Console.WriteLine(string.Format(
                "{0}: {1}:{2}.{3}", m, timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds
            ));
        }
    }
}
