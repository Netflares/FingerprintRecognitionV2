
using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class Segmentation
    {
        static public bool[,] CreateMask(double[,] norm, double avg, int bs)
        {
            int height = norm.GetLength(0), width = norm.GetLength(1);
            bool[,] res = new bool[height, width];
            double threshold = 0.2 * MatStatistic.Std(norm, avg);
            double bsSqr = bs * bs;

            Iterator2D.Forward(height / bs, width / bs, (i, j) =>
            {
                double avg = MatStatistic.SumBlock(norm, i, j, bs) / bsSqr;
                double std = MatStatistic.StdBlock(norm, avg, i, j, bs);
                if (std >= threshold)
                    Iterator2D.ForwardBlock(i, j, bs, (y, x) => res[y, x] = true);
                return 0;
            });

            // morphology here

            return res;
        }
    }
}
