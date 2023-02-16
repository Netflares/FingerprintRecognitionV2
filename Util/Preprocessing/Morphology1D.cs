
using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Morphology1D
    {
        static public void GrayDilation(double[] src, double[] res, int kernelSize, int v)
        {
            int r = kernelSize >> 1, len = src.Length;
            Deque<Pair<double, int>> q = new();    // q[i]: {value, index}
            
            for (int i = 0; i < len + r; i++) 
            {
                if (i < len) 
                {
                    while (q.Count > 0 && q.Last().St <= src[i]) q.PopBack();
                    q.PushBack(new(src[i], i));
                }
                if (i - q.First().Nd >= kernelSize) 
                    q.PopFront();
                if (i >= r) 
                    res[i - r] = q.First().St + v;
            }
        }
    }
}
