
using FingerprintRecognitionV2.Algorithm;
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
                    SpanIter.ForwardBlock(res, i, j, bs, true);
                return 0;
            });

            // morphology here

            return res;
        }

        static public void SmoothMask(bool[,] src, int bs)
        {
            MorphologyR4.Open(src, bs<<1);  // heavily segmentated
            MorphologyR4.Close(src, bs);
        }
    }
}
