
namespace FingerprintRecognitionV2.Util.Comparator
{
    /** 
     * the usage of this class is define in the doc:
     * "_doc/Improving Fingerprint Verification Using Minutiae Triplets.pdf"
     * */
    static public class Geometry
    {
        /** 
         * @ some constants
         * */
        static public readonly double 
            PI = Math.PI,
            TwoPI = Math.PI * 2,
            HalfPI = Math.PI / 2,
            ThreeHalfPI = PI + HalfPI;

        /** 
         * @ the core
         * */
        static public double AdPI(double a, double b)
        {
            double ans = Math.Abs(a - b);
            return Math.Min(ans, TwoPI - ans);
        }

        static public double Ad2PI(double a, double b)
        {
            double ans = b - a;
            return ans >= 0 ? ans : ans + TwoPI;
        }

        static public double Ang(int y0, int x0, int y1, int x1)
        {
            double dy = y1 - y0, dx = x1 - x0;
            if (dx == 0)
                return dy > 0 ? HalfPI : ThreeHalfPI;
            double ans = Math.Atan2(dy, dx);
            if (dx < 0) return ans + PI;
            if (dy < 0) return ans + TwoPI;
            return ans;
        }
    }
}
