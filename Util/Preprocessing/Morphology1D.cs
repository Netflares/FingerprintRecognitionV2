
namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Morphology1D
    {
        // replace this later
        static public double[] SimpleGrayDilation(double[] src, int kernelSize, int v)
        {
            int r = (kernelSize - 1) >> 1, l = src.Length;
            double[] res = new double[l];

            // init
            for (int i = 0; i < l; i++)
                res[i] = Double.MinValue;

            for (int i = 0; i < l; i++)
            {
                for (int j = Math.Max(0, i - r); j <= Math.Min(i + r, l - 1); j++)
                {
                    res[j] = Math.Max(res[j], src[i] + v);
                }
            }
            return res;
        }
    }
}
