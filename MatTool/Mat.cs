using System.Numerics;

namespace FingerprintRecognitionV2.MatTool
{
    public class Mat<T>
        where T : new()
    {
        public T[,] Arr;

        /** 
         * @ constructors
         * */
        protected Mat()
        {
            Arr = new T[0, 0];
        }

        /// <summary>
        /// Perform a SHALLOW COPY of `T[,] src`. Any modification in this wrapper will be written directly to `src`
        /// </summary>
        public Mat(T[,] src)
        {
            Arr = src;
            
        }

        public override string ToString()
        {
            string str = string.Format("Size = [{0}, {1}]\n", Arr.GetLength(0), Arr.GetLength(1));
            for (int y = 0; y < Arr.GetLength(0); y++)
            {
                for (int x = 0; x < Arr.GetLength(1); x++)
                    str += Arr[y, x] + " ";
                str += "\n";
            }
            return str;
        }

        /** 
         * @ dynamic operators
         * 
         * assume that two matrices in a binary operator have the same size
         * */
        public void And(bool[,] b)
        {
            for (int y = 0; y < b.GetLength(0); y++)
                for (int x = 0; x < b.GetLength(1); x++)
                    if (!b[y, x]) Arr[y, x] = new();
        }

        /** 
         * @ static operators
         * 
         * assume that two matrices in a binary operator have the same size
         * */
        static public Mat<T> operator &(Mat<T> a, bool[,] b)
        {
            Mat<T> res = new(a.Arr.Clone() as T[,]);
            res.And(b);
            return res;
        }
    }
}
