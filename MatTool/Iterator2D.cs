using System.Numerics;
using Emgu.CV;
using Emgu.CV.Structure;
using static System.Math;

namespace FingerprintRecognitionV2.MatTool
{
    static public class Iterator2D
    {
        /** 
         * @ core
         * */
        static public void Forward(int t, int l, int d, int r, Func<int, int, object> f)
        {
            for (int y = t; y < d; y++)
                for (int x = l; x < r; x++)
                    f(y, x);
        }

        static public void Forward(int h, int w, Func<int, int, object> f)
        {
            Forward(0, 0, h, w, f);
        }

        static public void ForwardBlock(int i, int j, int bs, Func<int, int, object> f)
        {
            int t = i * bs, l = j * bs;
            Forward(t, l, t + bs, l + bs, f);
        }

        /** 
         * @ core, but with return value
         * */
        static public TRes Sum<TRes>(int t, int l, int d, int r, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
        {
            // res = sigma{ f(y, x) }
            TRes res = new();
            for (int y = t; y < d; y++)
                for (int x = l; x < r; x++)
                    res += f(y, x);
            return res;
        }

        static public TRes Sum<TRes>(int h, int w, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
        {
            // res = sigma{ f(y, x) }
            TRes res = new();
            return Sum(0, 0, h, w, f);
        }

        /** 
         * @ 2d-array forward
         * */
        static public void Forward<T>(T[,] mat, Func<int, int, object> f)
        {
            Forward(0, 0, mat.GetLength(0), mat.GetLength(1), f);
        }

        static public void Forward<T>(T[,] mat, int t, int l, int d, int r, Func<int, int, object> f)
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(mat.GetLength(0), d);
            r = Min(mat.GetLength(1), r);
            Forward(t, l, d, r, f);
        }

        static public void ForwardBlock<T>(T[,] mat, int y, int x, int blockSize, Func<int, int, object> f) 
        {
            // iterate through the (y, x) block of the mat
            int t = y * blockSize;
            int l = x * blockSize;
            Forward(mat, t, l, t + blockSize, l + blockSize, f);
        }

        /** 
         * @ 2d-array sum
         * */
        static public TRes Sum<TRes, T>(T[,] mat, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
        {
            return Sum(0, 0, mat.GetLength(0), mat.GetLength(1), f);
        }

        static public TRes Sum<TRes, T>(T[,] mat, int t, int l, int d, int r, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(mat.GetLength(0), d);
            r = Min(mat.GetLength(1), r);
            return Sum(t, l, d, r, f);
        }

        static public TRes SumBlock<TRes, T>(T[,] mat, int y, int x, int blockSize, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
        {
            // iterate through the (y, x) block of the mat
            int t = y * blockSize;
            int l = x * blockSize;
            return Sum(mat, t, l, t + blockSize, l + blockSize, f);
        }

        /** 
         * @ emgu image forward
         * */
        static public void Forward<T>(Image<Gray, T> img, Func<int, int, object> f)
            where T : new()
        {
            Forward(img.Height, img.Width, f);
        }

        static public void Forward<T>(Image<Gray, T> img, int t, int l, int d, int r, Func<int, int, object> f)
            where T : new()
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(img.Height, d);
            r = Min(img.Width, r);
            Forward(t, l, d, r, f);
        }

        static public void ForwardBlock<T>(Image<Gray, T> img, int y, int x, int blockSize, Func<int, int, object> f) 
            where T : new()
        {
            // iterate through the (y, x) block of the img
            int t = y * blockSize;
            int l = x * blockSize;
            Forward(img, t, l, t + blockSize, l + blockSize, f);
        }

        /** 
         * @ emgu image sum
         * */
        static public TRes Sum<TRes, T>(Image<Gray, T> img, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
            where T : new()
        {
            return Sum(0, 0, img.Height, img.Width, f);
        }

        static public TRes Sum<TRes, T>(Image<Gray, T> img, int t, int l, int d, int r, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
            where T : new()
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(img.Height, d);
            r = Min(img.Width, r);
            return Sum(t, l, d, r, f);
        }

        static public TRes SumBlock<TRes, T>(Image<Gray, T> img, int y, int x, int blockSize, Func<int, int, TRes> f)
            where TRes : INumber<TRes>, new()
            where T : new()
        {
            // iterate through the (y, x) block of the mat
            int t = y * blockSize;
            int l = x * blockSize;
            return Sum(img, t, l, t + blockSize, l + blockSize, f);
        }
    }
}
