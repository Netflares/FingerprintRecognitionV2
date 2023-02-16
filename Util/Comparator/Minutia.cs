
namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Minutia
    {
        static public readonly byte NO_TYPE = 0, ENDING = 1, BIFUR = 2;

        /** 
         * `y` and `x` location are mapped to range [0: 256)
         * `Angle` is mapped from [0: 2PI) to [0: 256)
         * */
        public int Y;
        public int X;
        public double Angle;

        /** 
         * @ main constructors
         * */
        public Minutia(int y, int x, double angle)
        {
            Y = y;
            X = x;
            Angle = angle;
        }

        /** 
         * @ supporting tools
         * */
        public Minutia() { }

        public Minutia(int y, int x)
        {
            Y = y;
            X = x;
        }
    }
}
