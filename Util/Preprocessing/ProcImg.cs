using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;

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
        // distance between ridges is also a vital information
        public double WaveLen;

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
        static public bool[,] GaborMat = new bool[Height, Width];

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

            // wavelength (frequency)
            Normalization.ExcludeBackground(NormMat, SegmentMsk);
            WaveLen = Wavelength.GetMedianWavelength(NormMat, OrientMat, SegmentMsk);

            // gabor filter
            GaborFilter.Apply(NormMat, OrientMat, WaveLen, SegmentMsk, GaborMat);
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
