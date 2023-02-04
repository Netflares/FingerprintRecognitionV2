
namespace FingerprintRecognitionV2.Util.Comparator
{
    /** 
     * the usage of this class is define in the doc:
     * "_doc/Improving Fingerprint Verification Using Minutiae Triplets.pdf"
     * */
    static public class FastGeometry
    {
        /** 
         * @ some constants
         * */
        static public readonly int 
            PI = 128,
            TwoPI = 256,
            HalfPI = 64,
            ThreeHalfPI = PI + HalfPI;

        /** 
         * @ the core
         * */
        static public int AdPI(int a, int b)
        {
            int ans = Math.Abs(a - b);
            return Math.Min(ans, TwoPI - ans);
        }

        static public int Ad2PI(int a, int b)
        {
            int ans = b - a;
            return ans >= 0 ? ans : ans + TwoPI;
        }

        static public int Ang(int y0, int x0, int y1, int x1)
        {
            int dy = y1 - y0, dx = x1 - x0;
            if (dx == 0)
                return dy > 0 ? HalfPI : ThreeHalfPI;
            int ans = FastMath.Atan2(dy, dx);
            if (dx < 0) return ans + PI;
            if (dy < 0) return ans + TwoPI;
            return ans;
        }
    }
}
