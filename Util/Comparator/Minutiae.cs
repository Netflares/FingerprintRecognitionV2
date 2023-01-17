
namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Minutiae
    {
        static public readonly int NO_TYPE = 0, ENDING = 1, BIFUR = 2;

        public double T;    // type
        public double Y;    // y location
        public double X;    // x location
        public double A;    // the angle

        public Minutiae(int t, double y, double x, double a)
        {
            T = t;
            Y = y;
            X = x;
            A = a;
        }
    }
}
