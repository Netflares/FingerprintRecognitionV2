
using static System.Math;

namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class Geometry
    {
        /** 
         * @ some constants
         * */
        static readonly double V_2PI  = 2 * PI;
        static readonly double V_PI2  = PI / 2;
        static readonly double V_3PI2 = 3 * PI / 2;

        /** 
         * @ the core
         * */
        // calc the euclidean distance between 2 points
        static public double Dist(Minutia a, Minutia b)
        {
            return Sqrt(DistSqr(a, b));
        }

        // square the euclidean distance between 2 points
        static public double DistSqr(Minutia a, Minutia b)
        {
            return Sqr(a.Y - b.Y) + Sqr(a.X - b.X);
        }

        // calc the angle between 2 vector
        // given their angles `a` and `b`
        // return range: [0: PI]
        static public double Angle(double a, double b)
        {
            double ans = Abs(a - b);
            return Min(ans, V_2PI - ans);
        }

        // calc the clockwise angle
        // to superpose a vector with angle `a` to another vector with angle `b`
        // return range: [0: 2PI)
        static public double ClockwiseAngle(double a, double b)
        {
            double ans = b - a;
            return ans >= 0 ? ans : ans + PI;
        }

        // calc the angle between 2 points
        static public double Angle(Minutia a, Minutia b)
        {
            double dy = b.Y - a.Y, dx = b.X - a.X;
            if (dx == 0)
                return dy > 0 ? V_PI2 : V_3PI2; // dx = 0 & dy > 0 : dx = 0 & dy < 0
            double ans = Atan2(dy, dx);
            if (dx < 0) return ans + PI;        // dx < 0
            if (dy < 0) return ans + V_2PI;     // dx > 0 & dy < 0
            return ans;                         // dx > 0 & dy > 0
        }

        /** 
         * @ tools
         * */
        static private double Sqr(double x) => x * x;
    }
}
