using System.Numerics;
using static System.Math;

namespace FingerprintRecognitionV2.MatTool
{
    static public class MatOperator
    {
        /**
         * @ extracts data from mat
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

        static public double Std<T>(T[,] src, int t, int l, int d, int r)
            where T : INumber<T>, new()
        {
            double res = 0, avg = Avg<T>(src, t, l, d, r);
            int cnt = 0;
            Iterator2D.Forward(src, t, l, d, r, (y, x) =>
            {
                res += Sqr(Convert.ToDouble(src[y, x]) - avg);
                cnt++;
                return true;
            });
            return cnt == 0 ? 0 : Sqrt(res / cnt);
        }

        /** 
         * @ extracts data from mat - extension
         * */
        static public double Avg<T>(T[,] src)
            where T : INumber<T>, new()
        {
            return Avg(src, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        static public double Std<T>(T[,] src)
            where T : INumber<T>, new()
        {
            return Std(src, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        static public double Avg<T>(T[,] src, int y, int x, int bs)
            where T : INumber<T>, new()
        {
            // get avg value of block (y, x)
            int t = y * bs, l = x * bs;
            return Avg(src, t, l, t + bs, l + bs);
        }

        static public double Std<T>(T[,] src, int y, int x, int bs)
            where T : INumber<T>, new()
        {
            // get std value of block (y, x)
            int t = y * bs, l = x * bs;
            return Std(src, t, l, t + bs, l + bs);
        }

        /** 
         * @ modifies mat
         * */

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;
    }
}
