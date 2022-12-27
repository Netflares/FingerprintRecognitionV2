using System.Numerics;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FingerprintRecognitionV2.MatTool
{
    // this class serves debug purpose only
    // therefore it's not optimized
    static public class MatConverter
    {
        //
        static public Image<Gray, byte> Mat2Img<T>(T[,] src)
            where T : INumber<T>, new()
        {
            Image<Gray, byte> res = new(src.GetLength(1), src.GetLength(0));
            Iterator2D.Forward(src, (y, x) => 
                res[y, x] = new Gray(Convert.ToDouble(src[y, x]))
            );
            return res;
        }

        // from a binary matrix
        static public Image<Gray, byte> Bool2Img(bool[,] src)
        {
            Image<Gray, byte> res = new(src.GetLength(1), src.GetLength(0));
            Iterator2D.Forward(src, (y, x) => res[y, x] = new Gray(src[y, x] ? 255 : 0));
            return res;
        }

        // constrains around 0-255
        static public Image<Gray, byte> Double2Disp(double[,] src)
        {
            double mn = src[0, 0], mx = src[0, 0];
            Iterator2D.Forward(src, (y, x) =>
            {
                mn = mn < src[y, x] ? mn : src[y, x];
                mx = mx > src[y, x] ? mx : src[y, x];
                return true;
            });

            double span = mx - mn;
            Image<Gray, byte> res = new(src.GetLength(1), src.GetLength(0));
            Iterator2D.Forward(
                src, (y, x) => res[y, x] = new Gray((src[y, x] - mn) / span * 255)
            );
            return res;
        }

        static public Image<Gray, byte> Double2Disp(Image<Gray, double> src)
        {
            double[,] mat = new double[src.Height, src.Width];
            Iterator2D.Forward(mat, (y, x) => mat[y, x] = src[y, x].Intensity);
            return Double2Disp(mat);
        }
    }
}
