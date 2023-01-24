using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.Util.Comparator;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // `ProcImg`: The Image after pre-processing
    public class ProcImg
    {
        /** 
         * @ public attrs
         * */      
        // distance between ridges is also a vital information
        public double WaveLen;
        public List<Minutia> Minutiae;

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
        static public double[,] OrientMat = new double[Height / BlockSize, Width / BlockSize];
        static public bool[,] SkeletonMat = new bool[Height, Width];

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
            GaborFilter.Apply(NormMat, OrientMat, WaveLen, SegmentMsk, SkeletonMat);

            // skeletonization
            ZhangBruteThinning.Thinning(SkeletonMat);

            // extract informations
            /*
            MorphologyR4.Erose(SegmentMsk, BlockSize);
            Segmentation.Padding(SegmentMsk, BlockSize);
            Minutiae = BruteMinutiaeExtractor.Extract(SkeletonMat, OrientMat, SegmentMsk, (int) WaveLen, BlockSize);
            */
            Minutiae = MinutiaeExtractor.Extract(SkeletonMat, SegmentMsk, BlockSize);
        }

        public void Export(string fname)
        {
            using FileStream f = File.OpenWrite(fname);
            using StreamWriter o = new(f);
            foreach (Minutia i in Minutiae)
            {
                o.Write(i.T + " ");
                o.Write(i.Y + " ");
                o.Write(i.X + " ");
                o.Write(i.A + "\n");
            }
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

        public Image<Bgr, byte> Visualize()
        {
            Image<Bgr, byte> res = Visualization.Bool2Bgr(SkeletonMat);

            foreach (var i in Minutiae)
            {
                Visualization.DrawLine(res, (int)i.Y, (int)i.X, i.A, 12, 0, new Bgr(0, 0, 255));
                Visualization.Plot(res, (int)i.Y, (int)i.X, 2, new Bgr(255, 0, 0));
            }

            return res;
        }
    }
}
