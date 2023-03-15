using Emgu.CV;
using Emgu.CV.Structure;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Normalization
    {
        /** 
         * @ usage: remove finger pressure differences
         * @ result: avg(NormMat) = 0, std(NormMat) = 1
         * */
        static public void Normalize(Image<Gray, byte> src, double[,] res)
        {
            /*
                `src`:
                    the source image
                    white fg, black bg
                `res`:
                    the img after normalization
                    res[y, x] < 0 --> res[y, x] is foreground
                    res[y, x] > 0 --> res[y, x] is background
            */

            // std = sqrt( sum((src - avg)**2) / size )
            Gray avgObj;
            MCvScalar stdObj;
            src.AvgSdv(out avgObj, out stdObj);
            double avg = avgObj.Intensity, std = stdObj.V0;

            Parallel.For(0, Param.Height, (y) =>
            {
                for (int x = 0; x < Param.Width; x++)
                    res[y, x] = NormalizePixel(src[y, x].Intensity, avg, std);
            }); 
        }


        // takes a pixel and returns a normalized value
        static private double NormalizePixel(double px, double avg, double std)
        {
            /**
             * if your images have white foreground & black background
             * uncomment the second return statement & comment the first
             * 
             * otherwise, if your images have black foreground & white background
             * uncomment the first return statement & comment the second one
             * */
            // return (px - avg) / std;
            return (avg - px) / std;
        }

        /** 
         * @ result: ridge regions have std = 1
         * */
        unsafe static public void ExcludeBackground(double[,] srcMat, bool[,] mskMat)
        {
            /*
            this code but written in python:
                avg = np.mean(src[msk==0])
                std = np.std(src[msk==0])
                src = (src - avg) / std
            */

            double sum = 0, avg = 0, std = 0;
            int len = Param.Size, n = 0;

            Span<double> src;
            fixed (double* p = srcMat) src = new(p, len);
            Span<bool> msk;
            fixed (bool* p = mskMat) msk = new(p, len);

            for (int i = 0; i < len; i++) if (!msk[i]) { sum += src[i]; n++; }
            avg = sum / n;

            for (int i = 0; i < len; i++) if (!msk[i]) { double v = src[i] - avg; std += v * v; }
            std = Sqrt(std / n);

            foreach (ref double i in src) i = (i - avg) / std;
        }
    }
}
