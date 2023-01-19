
namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Minutia
    {
        static public readonly byte NO_TYPE = 0, ENDING = 1, BIFUR = 2;

        public byte T;      // type
        public double Y;    // y location
        public double X;    // x location
        public double A;    // the angle

        public Minutia(byte t, double y, double x, double a)
        {
            T = t;
            Y = y;
            X = x;
            A = a;
        }
    }
}
