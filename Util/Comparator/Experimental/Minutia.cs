namespace FingerprintRecognitionV2.Util.Comparator.Experimental
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

        public Minutia(double y, double x)
        {
            Y = y;
            X = x;
        }

        public override string ToString()
        {
            return string.Format("({0}; {1}) a={2} t={3}", Y, X, A, T);
        }
    }
}
