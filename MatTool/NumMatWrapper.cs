
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

        /** 
         * @ dynamic operators between matrices
         * 
         * assume that two matrices in a binary operator have the same size
         * */
        public void Mul<TOther>(TOther[,] v)
            where TOther : INumber<TOther>, new()
        {
            Merge(v, (a, b) => a * b);
        }

        public void Merge<TOther>(TOther[,] v, Func<T, T, T> f)
            where TOther : INumber<TOther>, new()
        {
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(1); x++)
                    Arr[y, x] = f(Arr[y, x], (T)(object)v[y, x]);
        }

        /** 
         * @ dynamic operators with a real value
         * */
        public void Mul<TOther>(TOther v)
            where TOther : INumber<TOther>, new()
        {
            Merge(v, (a, b) => a * b);
        }

        public void Merge<TOther>(TOther v, Func<T, T, T> f)
            where TOther : INumber<TOther>, new()
        {
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(1); x++)
                    Arr[y, x] = f(Arr[y, x], (T)(object) v);
        }

        /** 
         * @ static operators between matrices
         * */
        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a, T[,] b)
        {
            NumMatWrapper<T> res = new(a.Arr.Clone() as T[,]);
            res.Merge(b, (a, b) => a + b);
            return res;
        }

        static public NumMatWrapper<T> operator +(NumMatWrapper<T> a, NumMatWrapper<T> b)
        {
            return a + b.Arr;
        }
    }
}
