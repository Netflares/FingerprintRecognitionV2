using System.Numerics;
using static System.Math;

namespace FingerprintRecognitionV2.MatTool
{
    /** 
     * @ usage:
     * 
     * provide robust memory magic from C
     * 
     * these code will avoid inheritance and such thing as much as possible
     * copy-pasting is encouraged
     * */
    static public class MatStatistic
    {
        /** 
         * @ sum
         * */
        unsafe static public double Sum(double[,] src, int t, int l, int d, int r)
        {
            int h = src.GetLength(0), w = src.GetLength(1); // size of the matrix
            int len = h * w;        // size of the matrix
            int col = r - l;        // the width of the block
            int itr = t * w + l;    // the first pointer of this block
            int end = d * w;        // the itr won't go here

            double res = 0;         // the result

            fixed (double* p = src)
            {
                Span<double> span = new Span<double>(p, len);
                while (itr < end)
                {
                    Span<double> arr = span.Slice(itr, col);
                    foreach (double v in arr)
                        res += v;
                    itr += w;
                }
            }

            return res;
        }

        unsafe static public double Sum(double[,] src)
        {
            return Sum(src, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        unsafe static public double SumBlock(double[,] src, int y, int x, int bs)
        {
            // get std value of block (y, x)
            int t = y * bs, l = x * bs;
            return Sum(src, t, l, t + bs, l + bs);
        }

        /** 
         * @ std
         * */
        unsafe static public double Std(double[,] src, double avg, int t, int l, int d, int r)
        {
            int h = src.GetLength(0), w = src.GetLength(1); // size of the matrix
            int len = h * w;        // size of the matrix
            int col = r - l;        // the width of the block
            int itr = t * w + l;    // the first pointer of this block
            int end = d * w;        // the itr won't go here

            double std = 0;         // the result

            fixed (double* p = src)
            {
                Span<double> span = new(p, len);
                while (itr < end)
                {
                    Span<double> arr = span.Slice(itr, col);
                    foreach (double v in arr)
                        std += (v - avg) * (v - avg);
                    // jump to next row
                    itr += w;
                }
            }

            return Sqrt(std / ((d - t) * col));
        }

        unsafe static public double Std(double[,] src, double avg)
        {
            return Std(src, avg, 0, 0, src.GetLength(0), src.GetLength(1));
        }

        unsafe static public double StdBlock(double[,] src, double avg, int y, int x, int bs)
        {
            // get std value of block (y, x)
            int t = y * bs, l = x * bs;
            return Std(src, avg, t, l, t + bs, l + bs);
        }

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;
    }
}
