
using FingerprintRecognitionV2.DataStructure;

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
                for (int j = Math.Max(0, i - r); j <= Math.Min(i + r, l - 1); j++)
                    res[j] = Math.Max(res[j], src[i] + v);
            return res;
        }

        static public double[] GrayDilation(double[] src, int kernelSize, int v)
        {
            int r = kernelSize >> 1, len = src.Length;
            Deque<Pair<double, int>> q = new();    // q[i]: {value, index}
            double[] res = new double[len];
            
            for (int i = 0; i < len + r; i++) 
            {
                if (i < len) {
                    while (q.Count > 0 && q.Last().St <= src[i]) q.PopBack();
                    q.PushBack(new(src[i], i));
                }
                if (i - q.First().Nd >= kernelSize) 
                    q.PopFront();
                if (i >= r) 
                    res[i - r] = q.First().St + v;
            }

            return res;
        }
    }
}
