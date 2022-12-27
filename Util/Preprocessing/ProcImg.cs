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
        // these properties should be public
        static public readonly int Height = 480, Width = 320, ImgSize = Height * Width, BlockSize = 16;

        static public readonly double AVG0 = 100, STD0 = 100, SQRT_STD0 = 10;

        // these should be private
        static public double[,] NormMat = new double[Height, Width];

        /** 
         * @ pipline
         * */
        public ProcImg(Image<Gray, byte> src)
        {
            // remove finger pressure differences
            Normalization.Normalize(src, NormMat, AVG0, STD0);

            // segmentation
            SegmentMsk = Segmentation.CreateMask(NormMat, AVG0, BlockSize);
            Segmentation.SmoothMask(SegmentMsk, BlockSize);

            // orientation
            Normalization.SelfNormalize(NormMat, ImgSize, AVG0, SQRT_STD0);
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
