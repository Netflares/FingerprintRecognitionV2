using System.Numerics;
using static System.Math;

namespace FingerprintRecognitionV2.MatTool
{
    static public class MatStatistic
    {
        /** 
         * @ std
         * */
        unsafe static public double Std(double[,] src, int t, int l, int d, int r)
        {
            int cnt = (d - t) * (r - l);
            double avg = Iterator2D.Sum<double>(t, l, d, r, (y, x) => src[y, x]) / cnt;
            double std = Iterator2D.Sum<double>(t, l, d, r, (y, x) => Sqr(src[y, x] - avg));
            return Sqrt(std / cnt);
        }

        unsafe static public double Std(double[,] src)
        {
            return Std(src, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        unsafe static public double StdBlock(double[,] src, int y, int x, int bs)
        {
            // get std value of block (y, x)
            int t = y * bs, l = x * bs;
            return Std(src, t, l, t + bs, l + bs);
        }

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;
    }
}
