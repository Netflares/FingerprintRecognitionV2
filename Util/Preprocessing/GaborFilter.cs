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
            BS = ProcImg.BlockSize, OrientSize = ProcImg.ImgSize / BS / BS;

        static public void Apply(double[,] norm, double[,] orient, double waveLen, bool[,] msk, bool[,] res)
        {
            List<double> angles = CompressAngle(orient);
            List<Image<Gray, double>> img = CreateOrientFilter(
                norm, angles, CreateBaseFilter(waveLen)
            );

            Iterator2D.Forward(Height / BS, Width / BS, (i, j) =>
            {
                double ang = PI / 2 - orient[i, j];
                int angInd = LowerBound(angles, ang);

                Iterator2D.ForwardBlock(i, j, BS, (y, x) =>
                    res[y, x] = msk[y, x] && (img[angInd][y, x].Intensity < 0)
                );
            });
        }

        /**
         * @ rotate the base kernel for each orientation in `angles`
         * */
        static private List<Image<Gray, double>> CreateOrientFilter(double[,] norm, List<double> angles, Image<Gray, double> kernel)
        {
            Image<Gray, double> src = new(Width, Height);
            Iterator2D.Forward(src, (y, x) => src[y, x] = new Gray(norm[y, x]));    // can be paralleled

            List<Image<Gray, double>> res = new( new Image<Gray, double>[angles.Count] );
            Parallel.For(0, angles.Count, i =>
            {
                Image<Gray, double> filter = kernel.Rotate(-angles[i] * 180 / PI, new Gray(0));
                Image<Gray, double> img = new(Width, Height);
                CvInvoke.Filter2D(src, img, filter, new System.Drawing.Point(-1, -1));
                res[i] = img;
            });

            return res;
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
         * @ collection helpers
         * */
        static private List<double> AcceptedAngles = new();

        // compress the orient matrix into unique value set
        unsafe static private List<double> CompressAngle(double[,] orient)
        {
            // this is kinda awkward rn
            if (AcceptedAngles.Count == 0)
            {
                double angleInc = 3.0 * PI / 180.0;
                for (double i = -PI / 2.0; i <= PI / 2.0; i += angleInc)
                    AcceptedAngles.Add(i);
            }

            SortedSet<double> compressed = new();
            fixed (double* p = orient)
            {
                Span<double> arr = new(p, OrientSize);
                foreach (double v in arr)
                    compressed.Add(LowerBoundItem(AcceptedAngles, PI / 2 - v));
            }
            return new List<double>(compressed);
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

        /** 
         * @ calculators
         * */
        static private double Sqr(double x) => x * x;
    }
}
