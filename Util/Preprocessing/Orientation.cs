using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class Orientation
    {
        private double[,] 
            GX = new double[Param.Height, Param.Width], 
            GY = new double[Param.Height, Param.Width];

        SobelOperator sobel = new();

        public Orientation()
        {

        }

        /**
         * @ params
         * norm: the normalized image, size (Height * Width)
         * res:  the result image, size ((Height / BlockSIze) * (Width / BlockSize))
         * */
        public void Create(double[,] norm, double[,] res)
        {
            sobel.SobelY(norm, GY);
            sobel.SobelX(norm, GX);

            Iterator2D.Forward(res, (i, j) =>
            {
                double a = 0, b = 0;
                Iterator2D.ForwardBlock(norm, i, j, Param.BlockSize, (y, x) =>
                {
                    double vy = Round(GY[y, x]), vx = Round(GX[y, x]);
                    a += 2.0 * vy * vx;
                    b += vx * vx - vy * vy;
                });
                if (a != 0 || b != 0)
                    res[i, j] = (PI + Atan2(a, b)) / 2;
                else 
                    res[i, j] = 0;
            });
        }
    }
}
