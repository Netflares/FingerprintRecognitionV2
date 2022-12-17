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
        static public void Forward(int t, int l, int d, int r, Func<int, int, bool> f)
        {
            for (int y = t; y < d; y++)
                for (int x = l; x < r; x++)
                    f(y, x);
        }

        static public void Forward(int h, int w, Func<int, int, bool> f)
        {
            Forward(0, 0, h, w, f);
        }

        /** 
         * @ 2d-array
         * */
        static public void Forward<T>(T[,] mat, Func<int, int, bool> f)
        {
            Forward(0, 0, mat.GetLength(0), mat.GetLength(1), f);
        }

        static public void Forward<T>(T[,] mat, int t, int l, int d, int r, Func<int, int, bool> f)
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(mat.GetLength(0), d);
            r = Min(mat.GetLength(1), r);
            Forward(t, l, d, r, f);
        }

        static public void ForwardBlock<T>(T[,] mat, int y, int x, int blockSize, Func<int, int, bool> f) 
        {
            // iterate through the (y, x) block of the mat
            int t = y * blockSize;
            int l = x * blockSize;
            Forward(mat, t, l, t + blockSize, l + blockSize, f);
        }

        /** 
         * @ emgu image
         * */
        static public void Forward<T>(Image<Gray, T> img, Func<int, int, bool> f)
            where T : new()
        {
            Forward(img.Height, img.Width, f);
        }

        static public void Forward<T>(Image<Gray, T> img, int t, int l, int d, int r, Func<int, int, bool> f)
            where T : new()
        {
            t = Max(0, t);
            l = Max(0, l);
            d = Min(img.Height, d);
            r = Min(img.Width, r);
            Forward(t, l, d, r, f);
        }

        static public void ForwardBlock<T>(Image<Gray, T> img, int y, int x, int blockSize, Func<int, int, bool> f) 
            where T : new()
        {
            // iterate through the (y, x) block of the img
            int t = y * blockSize;
            int l = x * blockSize;
            Forward(img, t, l, t + blockSize, l + blockSize, f);
        }
    }
}
