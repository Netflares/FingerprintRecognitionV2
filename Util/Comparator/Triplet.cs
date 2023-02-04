namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Triplet
    {
        public Minutia[] Minutiae;
        public int[] Distances;

        public Triplet(Minutia m0, Minutia m1, Minutia m2)
        {
            Minutiae = new Minutia[3] { m0, m1, m2 };
            ShiftClockwise(Minutiae);
            Distances = CalcSortedDist(Minutiae);
        }

        static public bool operator <(Triplet a, Triplet b) => a.Distances[2] < b.Distances[2];
        static public bool operator >(Triplet a, Triplet b) => a.Distances[2] > b.Distances[2];

        /*
        returns { dMin, dMid, dMax }
        m.Length = 3
        */
        static public int[] CalcSortedDist(Minutia[] m)
        {
            static int d(Minutia a, Minutia b)
            {
                int y = a.Y - b.Y, x = a.X - b.X;
                return FastMath.Sqrt(y * y + x * x);
            }

            int[] res = new int[3]{
                d(m[0], m[1]), d(m[1], m[2]), d(m[0], m[2])
            };
            Array.Sort(res);
            return res;
        }

        /*
        clockwise sorts the Minutiae set
        m.Length = 3
        */
        static public void ShiftClockwise(Minutia[] m)
        {
            int cy = ((m[0].Y + m[1].Y) / 2 + m[2].Y) / 2;
            int cx = ((m[0].X + m[1].X) / 2 + m[2].X) / 2;

            int a0 = FastMath.Atan2(m[0].Y - cy, m[0].X - cx);
            int a1 = FastMath.Atan2(m[1].Y - cy, m[1].X - cx);
            int a2 = FastMath.Atan2(m[2].Y - cy, m[2].X - cx);

            if (a0 > a2)
            {
                Swap(ref a0, ref a2);
                Swap(ref m[0], ref m[2]);
            }
            if (a0 > a1)
            {
                Swap(ref a0, ref a1);
                Swap(ref m[0], ref m[1]);
            }
            if (a1 > a2)
            {
                Swap(ref a1, ref a2);
                Swap(ref m[1], ref m[2]);
            }
        }

        static public void Swap(ref Minutia a, ref Minutia b)
        {
            Minutia copy = a;
            a = b;
            b = copy;
        }

        static public void Swap(ref int a, ref int b)
        {
            int copy = a;
            a = b;
            b = copy;
        }
    }
}
