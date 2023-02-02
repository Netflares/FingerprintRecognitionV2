using static System.Math;

namespace FingerprintRecognitionV2.Util.Comparator.Experimental
{
    public class Triplet
    {
        public Minutia[] Minutiae;
        public double[] Distances;  // ascending 

        public Triplet(Minutia[] m)
        {
            Minutiae = m;
            ShiftClockwise(Minutiae);
            Distances = CalcSortedDist(Minutiae);
        }

        public Triplet()
        {

        }

        static public bool operator <(Triplet a, Triplet b) => a.Distances[2] < b.Distances[2];

        static public bool operator >(Triplet a, Triplet b) => a.Distances[2] > b.Distances[2];

        static public bool operator <=(Triplet a, Triplet b) => a.Distances[2] <= b.Distances[2];

        static public bool operator >=(Triplet a, Triplet b) => a.Distances[2] >= b.Distances[2];

        /*
        returns { dMax, dMid, dMin }
        m.Length = 3
        */
        static public double[] CalcSortedDist(Minutia[] m)
        {
            double[] res = new double[3]{
                Geometry.Dist(m[0], m[1]), Geometry.Dist(m[1], m[2]), Geometry.Dist(m[0], m[2])
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
            double cx = ((m[0].X + m[1].X) / 2 + m[2].X) / 2;
            double cy = ((m[0].Y + m[1].Y) / 2 + m[2].Y) / 2;

            double a0 = Atan2(m[0].Y - cy, m[0].X - cx);
            double a1 = Atan2(m[1].Y - cy, m[1].X - cx);
            double a2 = Atan2(m[2].Y - cy, m[2].X - cx);

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

        static public void Swap(ref double a, ref double b)
        {
            double copy = a;
            a = b;
            b = copy;
        }
    }
}
