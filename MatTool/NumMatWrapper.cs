
using System.Numerics;

namespace FingerprintRecognitionV2.MatTool
{
    public class NumMatWrapper<T> : MatWrapper<T>
        where T : INumber<T>, new()
    {
        /// <summary>
        /// Perform a SHALLOW COPY of `T[,] src`. Any modification in this wrapper will be written directly to `src`
        /// </summary>
        public NumMatWrapper(T[,] src)
        {
            Arr = src;
        }

        public NumMatWrapper(NumMatWrapper<T> src)
        {
            Arr = src.Arr.Clone() as T[,];
        }

        /** 
         * @ dynamic operators with a real value
         * */
        public void Mul(T v)
        {
            Merge(v, (a, b) => a * b);
        }

        public void Merge(T v, Func<T, T, T> f)
        {
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(1); x++)
                    Arr[y, x] = f(Arr[y, x], v);
        }

        /** 
         * @ dynamic operators between matrices
         * 
         * assume that two matrices in a binary operator have the same size
         * */
        public void Mul(T[,] v)
        {
            Merge(v, (a, b) => a * b);
        }

        public void Merge(T[,] v, Func<T, T, T> f)
        {
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(1); x++)
                    Arr[y, x] = f(Arr[y, x], v[y, x]);
        }

        /** 
         * @ static operators with a real value
         * */
        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a)
            => new(a);
        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a, T v)
        {
            NumMatWrapper<T> res = new(a);
            res.Merge(v, (i, v) => i + v);
            return res;
        }

        static public NumMatWrapper<T> operator -(NumMatWrapper<T> a)
        {
            NumMatWrapper<T> res = new(a);
            Iterator2D.Forward<T>(res.Arr, (y, x) => res.Arr[y, x] = -res.Arr[y, x]);
            return res;
        }
        static public NumMatWrapper<T> operator -(NumMatWrapper<T> a, T v)
        {
            NumMatWrapper<T> res = new(a);
            res.Merge(v, (i, v) => i - v);
            return res;
        }

        static public NumMatWrapper<T> operator *(NumMatWrapper<T> a, T v)
        {
            NumMatWrapper<T> res = new(a);
            res.Mul(v);
            return res;
        }

        /** 
         * @ static operators between matrices
         * */
        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a, T[,] b)
        {
            NumMatWrapper<T> res = new(a);
            res.Merge(b, (i, j) => i + j);
            return res;
        }
        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a, NumMatWrapper<T> b)
        {
            return a + b.Arr;
        }

        static public NumMatWrapper<T> operator -(NumMatWrapper<T> a, T[,] b)
        {
            NumMatWrapper<T> res = new(a);
            res.Merge(b, (i, j) => i + j);
            return res;
        }
        static public NumMatWrapper<T> operator -(NumMatWrapper<T> a, NumMatWrapper<T> b)
        {
            return a - b.Arr;
        }

        static public NumMatWrapper<T> operator *(NumMatWrapper<T> a, T[,] b)
        {
            NumMatWrapper<T> res = new(a);
            res.Mul(b);
            return res;
        }
        static public NumMatWrapper<T> operator *(NumMatWrapper<T> a, NumMatWrapper<T> b)
        {
            return a * b.Arr;
        }
    }
}
