using System.Diagnostics;
using DelaunatorSharp;
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
        public List<Triplet> Triplets;

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
            // starting from this part, the code is not optimized
            MorphologyR4.Erose(SegmentMsk, BlockSize);
            Segmentation.Padding(SegmentMsk, BlockSize);
            Minutiae = MinutiaeExtractor.Extract(SkeletonMat, SegmentMsk, BlockSize);

            // build m-triplets
            List<IPoint> pts = new(Minutiae.Count);
            foreach (Minutia m in Minutiae) pts.Add(new Point(m.X, m.Y));
            Delaunator d = new(pts.ToArray());
            int[] t = d.Triangles;

            Triplets = new(t.Length / 3);
            for (int i = 0; i + 2 < t.Length; i += 3)
            {
                Triplets.Add(new Triplet(new Minutia[3]
                {
                    Minutiae[i + 0], Minutiae[i + 1], Minutiae[i + 2]
                }));
            }

            Triplets.Sort();
        }

        public int LowerBound(double len)
        {
            int l = 0, r = Triplets.Count;
            while (l < r)
            {
                int m = (l + r + 0) >> 1;
                if (Triplets[m].Distances[2] >= len)
                    r = m;
                else
                    l = m + 1;
            }
            return r;
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

        static public Image<Bgr, byte> Visualize(bool[,] ske, List<Minutia> minutiae)
        {
            Image<Bgr, byte> res = Visualization.Bool2Bgr(ske);

            foreach (var i in minutiae)
            {
                Bgr color = new (i.T == Minutia.ENDING ? 255 : 0, i.T == Minutia.BIFUR ? 255 : 0, 0);

                Visualization.DrawLine(res, (int)i.Y, (int)i.X, i.A, 12, 0, new Bgr(0, 0, 255));
                Visualization.Plot(res, (int)i.Y, (int)i.X, 2, color);
            }

            return res;
        }
    }
}
