using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.PreprocessingMultithread
{
    public class GaborFilter
    {
        /** 
         * @ static values
         * */
        static private readonly int 
            Height = Param.Height, 
            Width = Param.Width, 
            ImgSize = Param.Size,
            BS = Param.BlockSize, 
            OrientSize = Param.Size / (BS * BS),
            KernelCount = (int) Round(PI / Param.AngleInc) + 1;

        static private List<double> AcceptedAngles = new(KernelCount);

        static GaborFilter()
        {
            for (double i = -PI / 2.0; i <= PI / 2.0; i += Param.AngleInc)
                AcceptedAngles.Add(i);
        }

        /** 
         * @ contructors
         * */
        List<Image<Gray, double>> Convol = new(KernelCount);

        public GaborFilter()
        {
            for (int i = 0; i < KernelCount; i++)
                Convol.Add(new Image<Gray, double>(Width, Height));
        }

        public void Apply(double[,] norm, double[,] orient, double waveLen, bool[,] msk, bool[,] res)
        {
            CreateOrientFilter(norm, CreateBaseFilter(waveLen), CompressAngle(orient));

            Iterator2D.Forward(Height / BS, Width / BS, (i, j) =>
            {
                double ang = PI / 2 - orient[i, j];
                int angInd = LowerBound(AcceptedAngles, ang);

                Iterator2D.ForwardBlock(i, j, BS, (y, x) =>
                    res[y, x] = msk[y, x] && (Convol[angInd][y, x].Intensity < 0)
                );
            });
        }

        /**
         * @ rotate the base kernel for each orientation in `angles`
         * */
        private void CreateOrientFilter(double[,] norm, Image<Gray, double> kernel, List<int> angleIndexes)
        {
            Image<Gray, double> src = new(Width, Height);
            Iterator2D.PForward(src, (y, x) => src[y, x] = new Gray(norm[y, x]));

            Parallel.For(0, angleIndexes.Count, i =>
            {
                Image<Gray, double> filter = kernel.Rotate(-AcceptedAngles[ angleIndexes[i] ] * 180 / PI, new Gray(0));
                CvInvoke.Filter2D(src, Convol[ angleIndexes[i] ], filter, new System.Drawing.Point(-1, -1));
            });
        }

        /** 
         * @ the base kernel for gabor filter
         * */
        static private Image<Gray, double> CreateBaseFilter(double wave)
        {
            double freq = 1.0 / wave;
            double sigma = wave * 0.65;
            int kr = (int)Round(3.0 * sigma);   // kernel radius
            int ks = kr << 1 | 1;               // kernel size

            double[,] mx = new double[ks, ks];
            double[,] my = new double[ks, ks];

            // ks = kr << 1 | 1
            Iterator2D.Forward(ks, ks, (y, x) =>
            {
                mx[y, x] = x - kr;
                my[y, x] = y - kr;
            });

            // gabor filter kernel equation:
            //      refa = - ( mx/(sigma**2) + my/(sigma**2) ) / 2
            //      refb = 2 * PI * waveLen * mx
            //      kernel = exp(refa) * cos(refb)
            Image<Gray, double> kernel = new(ks, ks);
            double sig2 = sigma * sigma, wpi = 2 * PI * freq;   // store what can be store (I don't trust O3)
            Iterator2D.Forward(ks, ks, (y, x) =>
            {
                kernel[y, x] = new Gray(
                    Exp(- (Sqr(mx[y, x]) / sig2 + Sqr(my[y, x]) / sig2)) * Cos(wpi * mx[y, x])
                );
            });

            return kernel;
        }

        /** 
         * @ tools
         * */
        // compress the orient matrix into unique value set
        unsafe static private List<int> CompressAngle(double[,] orient)
        {
            SortedSet<int> compressed = new();
            fixed (double* p = orient)
            {
                Span<double> arr = new(p, OrientSize);
                foreach (double v in arr)
                    compressed.Add(LowerBound(AcceptedAngles, PI / 2 - v));
            }
            return new List<int>(compressed);
        }

        // find the index of value `x` in `a`
        // only used in this class only
        static private int LowerBound(List<double> a, double x)
        {
            int l = 0, r = a.Count - 1;    // there's no a.end() returned
            while (l < r)
            {
                int m = (l + r + 0) >> 1;
                if (a[m] >= x)
                    r = m;
                else
                    l = m + 1;
            }
            return r;
        }

        static private double LowerBoundItem(List<double> a, double x)
        {
            return a[LowerBound(a, x)];
        }

        static private double Sqr(double x) => x * x;
    }
}
