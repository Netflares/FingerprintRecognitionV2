using System.Numerics;
using static System.Math;

namespace FingerprintRecognitionV2.MatTool
{
    static public class MatStatistic
    {
        /**
         * @ avg
         * */
        static public double Avg<T>(T[,] src, int t, int l, int d, int r)
            where T : INumber<T>, new()
        {
            double res = 0;
            int cnt = 0;
            Iterator2D.Forward(src, t, l, d, r, (y, x) =>
            {
                res += Convert.ToDouble(src[y, x]);
                cnt++;
                return true;
            });
            return cnt == 0 ? 0 : res / cnt;
        }

        static public double Avg<T>(T[,] src)
            where T : INumber<T>, new()
        {
            return Avg(src, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        static public double Avg<T>(T[,] src, int y, int x, int bs)
            where T : INumber<T>, new()
        {
            // get avg value of block (y, x)
            int t = y * bs, l = x * bs;
            return Avg(src, t, l, t + bs, l + bs);
        }

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;
    }
}
