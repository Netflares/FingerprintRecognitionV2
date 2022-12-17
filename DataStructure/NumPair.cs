using System.Numerics;

namespace FingerprintRecognitionV2.DataStructure
{
    // this class is heavily experimental
    // both to me and to dotnet as well
    public class NumPair<A, B> : Pair<A, B>
        where A : INumber<A>, new()
        where B : INumber<B>, new()
    {
        /** 
         * @ constructors
         * */
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
         * @ overridings
         * */
        public override bool Equals(object? obj)
        {
            if (obj == null || St == null || Nd == null || !this.GetType().Equals(obj.GetType()))
                return false;
            NumPair<A, B> x = (NumPair<A, B>)obj;
            return this == x;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /** 
         * @ operators
         * */
        static public NumPair<A, B> operator +(NumPair<A, B> a) 
            => new(a.St, a.Nd);

        static public NumPair<A, B> operator -(NumPair<A, B> a) 
            => new(-a.St, -a.Nd);

        static public NumPair<A, B> operator +(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St + b.St, a.Nd + b.Nd);

        static public NumPair<A, B> operator -(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St - b.St, a.Nd - b.Nd);

        static public NumPair<A, B> operator *(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St * b.St, a.Nd * b.Nd);

        static public NumPair<A, B> operator /(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St / b.St, a.Nd / b.Nd);

        static public NumPair<A, B> operator %(NumPair<A, B> a, NumPair<A, B> b)
            => new(a.St % b.St, a.Nd % b.Nd);

        /** 
         * @ comparison operator
         * */
        static public bool operator ==(NumPair<A, B> a, NumPair<A, B> b)
            => a.St == b.St && a.Nd == b.Nd;

        static public bool operator !=(NumPair<A, B> a, NumPair<A, B> b)
            => !(a == b);

        static public bool operator <(NumPair<A, B> a, NumPair<A, B> b)
        {
            if (a.St == b.St)
                return a.Nd < b.Nd;
            return a.St < b.St;
        }

        static public bool operator <=(NumPair<A, B> a, NumPair<A, B> b)
            => (a < b) && (a == b);

        static public bool operator >(NumPair<A, B> a, NumPair<A, B> b)
            => !(a <= b);

        static public bool operator >=(NumPair<A, B> a, NumPair<A, B> b)
            => !(a < b);
    }
}
