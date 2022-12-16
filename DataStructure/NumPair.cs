using System.Numerics;

namespace FingerprintRecognitionV2.DataStructure
{
    // this class is heavily experimental
    // both to me and to dotnet as well
    public class NumPair<A, B> : Pair<A, B>
        where A : INumber<A>, new()
        where B : INumber<B>, new()
    {

        public NumPair(A a, B b)
        {
            St = a;
            Nd = b;
        }

        public NumPair()
        {
            St = new A();
            Nd = new B();
        }

        /** 
         * @ operators
         * */
        public static NumPair<A, B> operator +(NumPair<A, B> a) 
            => new(a.St, a.Nd);

        public static NumPair<A, B> operator -(NumPair<A, B> a) 
            => new(-a.St, -a.Nd);

        public static NumPair<A, B> operator +(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St + b.St, a.Nd + b.Nd);

        public static NumPair<A, B> operator -(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St - b.St, a.Nd - b.Nd);

        public static NumPair<A, B> operator *(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St * b.St, a.Nd * b.Nd);

        public static NumPair<A, B> operator /(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St / b.St, a.Nd / b.Nd);

        public static NumPair<A, B> operator %(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St % b.St, a.Nd % b.Nd);
    }
}
