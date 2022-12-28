using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
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
        // size: (Height / BlockSize) * (Width / BlockSize)
        // keep this for Singularity detector
        public double[,] OrientMat;

        /** 
         * @ shared attrs
         * 
         * to optimize memory resources
         * */
        // these properties should be public
        static public readonly int Height = 480, Width = 320, ImgSize = Height * Width, BlockSize = 16;

        static public readonly double AVG0 = 100;

        // these should be private
        static public double[,] NormMat = new double[Height, Width];
        static public bool[,] SegmentMsk = new bool[Height, Width];

        /** 
         * @ pipline
         * */
        public ProcImg(Image<Gray, byte> src)
        {
            // @ usage: remove finger pressure differences
            // @ result: avg(NormMat) = 0, std(NormMat) = 1
            Normalization.Normalize(src, NormMat);

            // segmentation
            SegmentMsk = Segmentation.CreateMask(NormMat, BlockSize);
            Segmentation.SmoothMask(SegmentMsk, BlockSize);

            // orientation
            OrientMat = Orientation.Create(NormMat);
        }

        /** 
         * @ debug
         * */
        static public void PrintTime(Stopwatch timer, string m)
        {
            Console.WriteLine(string.Format(
                "{0}: {1}:{2}.{3}", m, 
                timer.Elapsed.Minutes, 
                timer.Elapsed.Seconds.ToString("D2"), 
                timer.Elapsed.Milliseconds.ToString("D3")
            ));
        }
    }
}
