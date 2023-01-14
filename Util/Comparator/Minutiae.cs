
namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Minutiae
    {
        public double Y;    // y location
        public double X;    // x location
        public double A;    // the angle

        public Minutiae(double y, double x, double a)
        {
            Y = y;
            X = x;
            A = a;
        }
    }
}
