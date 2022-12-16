
using System.Numerics;

namespace FingerprintRecognitionV2.DataStructure
{
    public class IntPair<A, B> : NumPair<A, B>
        where A: IBinaryInteger<A>, new()
        where B: IBinaryInteger<B>, new()
    {

        public IntPair(A a, B b)
        {
            St = a;
            Nd = b;
        }

        public IntPair()
        {
            St = new A();
            Nd = new B();
        }

        /** 
         * @ operators
         * */
        public static IntPair<A, B> operator ~(IntPair<A, B> a)
            => new(~a.St, ~a.Nd);

        public static IntPair<A, B> operator &(IntPair<A, B> a, IntPair<A, B> b)
            => new(a.St & b.St, a.Nd & b.Nd);

        public static IntPair<A, B> operator |(IntPair<A, B> a, IntPair<A, B> b)
            => new(a.St | b.St, a.Nd | b.Nd);

        public static IntPair<A, B> operator ^(IntPair<A, B> a, IntPair<A, B> b)
            => new(a.St ^ b.St, a.Nd ^ b.Nd);
    }
}