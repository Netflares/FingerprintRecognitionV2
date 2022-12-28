
using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Orientation
    {
        static private int 
            SH = ProcImg.Height, SW = ProcImg.Width,                    // src height, src width
            DH = SH / ProcImg.BlockSize, DW = SW / ProcImg.BlockSize;   // des height, des width

        /** @ static memory */
        static private double[,] 
            GX = new double[SH, SW], 
            GY = new double[SH, SW];

        static public double[,] Create(double[,] norm, int bs)
        {
            SobelOperator.SobelY(norm, GY);
            SobelOperator.SobelX(norm, GX);

            double[,] res = new double[DH, DW];
            Iterator2D.Forward(res, (i, j) =>
            {
                double a = 0, b = 0;
                Iterator2D.ForwardBlock(norm, i, j, bs, (y, x) =>
                {
                    double vy = GY[y, x], vx = GX[y, x];
                    a += 2.0 * vy * vx;
                    b += vx * vx - vy * vy;
                    return true;
                });
                if (a != 0 || b != 0)
                    res[i, j] = (PI + Atan2(a, b)) / 2;
                return true;
            });

            return res;
        }
    }
}
