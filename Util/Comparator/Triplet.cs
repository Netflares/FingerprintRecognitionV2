using static System.Math;

namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Triplet
    {
        static public void ShiftClockwise(Minutiae[] m)
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

        static public void Swap(ref Minutiae a, ref Minutiae b)
        {
            Minutiae copy = a;
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
