using System.Numerics;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FingerprintRecognitionV2.MatTool
{
    static public class MatConverter
    {
        // from a matrix to a display image
        static public Image<Gray, byte> Mat2Disp<T>(T[,] src)
            where T : INumber<T>, new()
        {
            Image<Gray, byte> res = new(src.GetLength(1), src.GetLength(0));
            Iterator2D.Forward(src, (y, x) => 
                res[y, x] = new Gray(Convert.ToDouble(src[y, x]))
            );
            return res;
        }

        // from a binary matrix
        static public Image<Gray, byte> Bool2Disp(bool[,] src)
        {
            Image<Gray, byte> res = new(src.GetLength(1), src.GetLength(0));
            Iterator2D.Forward(src, (y, x) => res[y, x] = new Gray(src[y, x] ? 255 : 0));
            return res;
        }
    }
}
