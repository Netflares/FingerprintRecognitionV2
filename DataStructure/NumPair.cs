using System.Numerics;

namespace FingerprintRecognitionV2.DataStructure
{
    // this class is heavily experimental
    // both to me and to dotnet as well
    public class NumPair<A, B>
        where A: INumber<A>, new()
        where B: INumber<B>, new()
    {
        public A St;
        public B Nd;

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

        public override string ToString()
        {
            return string.Format("({0}, {1})", St, Nd);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || St == null || Nd == null || !this.GetType().Equals(obj.GetType()))
                return false;
            Pair<A, B> x = (Pair<A, B>)obj;
            return St.Equals(x.St) && Nd.Equals(x.Nd);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
