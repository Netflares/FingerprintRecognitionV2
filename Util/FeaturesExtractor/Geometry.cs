using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Comparator.Experimental;
using static System.Math;

/** 
 * @ desc:
 * this static class provides equations
 * that appear in "_doc/Improving Fingerprint Verification Using Minutiae Triplets.pdf"
 * 
 * @ todo:
 * change the calculation
 * based on radian -> based on range [0:256)
 * */
namespace FingerprintRecognitionV2.Util.FeaturesExtractor
{
    static public class Geometry
    {
        /** 
         * @ some constants
         * */
        static public readonly double V_2PI = 2 * PI;
        static public readonly double V_PI2 = PI / 2;
        static public readonly double V_3PI2 = 3 * PI / 2;

        /** 
         * @ the core
         * */
        // calc the angle between 2 vector
        // given their angles `a` and `b`
        // return range: [0: PI]
        static public double AdPI(double a, double b)
        {
            double ans = Abs(a - b);
            return Min(ans, V_2PI - ans);
        }

        // calc the clockwise angle
        // to superpose a vector with angle `a` to another vector with angle `b`
        // return range: [0: 2PI)
        static public double Ad2PI(double a, double b)
        {
            double ans = b - a;
            return ans >= 0 ? ans : ans + V_2PI;
        }

        // ang() in the doc
        static public double Ang(double y0, double x0, double y1, double x1)
        {
            double dy = y1 - y0, dx = x1 - x0;
            if (dx == 0)
                return dy > 0 ? V_PI2 : V_3PI2; // dx = 0 & dy > 0 : dx = 0 & dy < 0
            double ans = Atan2(dy, dx);
            if (dx < 0) return ans + PI;        // dx < 0
            if (dy < 0) return ans + V_2PI;     // dx > 0 & dy < 0
            return ans;                         // dx > 0 & dy > 0
        }

        // calc geometry angle
        static public double Alpha(int y0, int x0, int y1, int x1)
        {
            double dy = y1 - y0, dx = x1 - x0;
            double len = Sqrt(dy * dy + dx * dx);

            double ans = Acos(dx / len);
            return dy >= 0 ? ans : V_2PI - ans;
        }
    }
}
