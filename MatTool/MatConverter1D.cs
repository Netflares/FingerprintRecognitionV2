
using Emgu.CV.Structure;
using Emgu.CV;
using System.Numerics;

namespace FingerprintRecognitionV2.MatTool
{
    static public class MatConverter1D
    {
        // 
        static public Image<Gray, byte> Mat2Img<T>(T[] src, int h, int w)
            where T : INumber<T>, new()
        {
            Image<Gray, byte> res = new(w, h);
            int y = 0, x = 0;
            for (int i = 0; i < src.Length; i++)
            {
                res[y, x] = new Gray(Convert.ToDouble(src[i]));
                if (++x == w)
                {
                    x = 0;
                    y++;
                }
            }
            return res;
        }
    }
}
