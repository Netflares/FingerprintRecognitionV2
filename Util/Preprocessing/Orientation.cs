using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class Orientation
    {
        private static int BlockArea = Param.BlockSize * Param.BlockSize;

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
         * res:  an orientation image, size ((Height / BlockSize) * (Width / BlockSize))
         * ocl:  Orientation Certainty Level, same size as `res`; documented here:
         *       https://www.hindawi.com/journals/misy/2015/401975
         * */
        public void Create(double[,] norm, double[,] res, double[,] ocl)
        {
            sobel.SobelY(norm, GY);
            sobel.SobelX(norm, GX);

            /**
             * anisotropy orientation estimation, equation (5) of:
             * https://pdfs.semanticscholar.org/6e86/1d0b58bdf7e2e2bb0ecbf274cee6974fe13f.pdf
             * */
            Iterator2D.Forward(res, (i, j) =>
            {
                double a = 0, b = 0;
                Iterator2D.ForwardBlock(norm, i, j, Param.BlockSize, (y, x) =>
                {
                    double dy = Round(GY[y, x]), dx = Round(GX[y, x]);
                    a += 2.0 * dy * dx;
                    b += dx * dx - dy * dy;
                });
                if (a != 0 || b != 0)
                    res[i, j] = (PI + Atan2(a, b)) / 2;
                else 
                    res[i, j] = 0;
            });

            /**
             * orientation certainty level calculation, equations (1)(2)(3) of:
             * https://www.hindawi.com/journals/misy/2015/401975
             * */
            Iterator2D.Forward(ocl, (i, j) => 
            {
                double a = 0, b = 0, c = 0;
                Iterator2D.ForwardBlock(norm, i, j, Param.BlockSize, (y, x) =>
                {
                    double dy = GY[y, x], dx = GX[y, x];
                    a += dx * dx;
                    b += dy * dy;
                    c += dy * dx;
                });
                a /= BlockArea;
                b /= BlockArea;
                c /= BlockArea;
                // about this "eigenvalues of covariance matrix":
                // https://math.stackexchange.com/questions/23596/why-is-the-eigenvector-of-a-covariance-matrix-equal-to-a-principal-component
                double sumPart = a + b, sqrtPart = Sqrt((a-b)*(a-b) + 4*c*c);
                ocl[i, j] = (sumPart - sqrtPart) / (sumPart + sqrtPart);
            });
        }
    }
}
