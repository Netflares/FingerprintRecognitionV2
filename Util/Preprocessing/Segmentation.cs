using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class Segmentation
    {
        static public bool[,] CreateMask(double[,] norm, int bs)
        {
            int height = norm.GetLength(0), width = norm.GetLength(1);
            bool[,] res = new bool[height, width];
            double bsSqr = bs * bs;

            int mxI = height / bs, mxJ = width / bs;
            Parallel.For(0, mxI, (i) =>
            {
                for (int j = 0; j < mxJ; j++)
                {
                    double avg = MatStatistic.SumBlock(norm, i, j, bs) / bsSqr;
                    double std = MatStatistic.StdBlock(norm, avg, i, j, bs);
                    if (std >= 0.2 || avg < -0.2)
                        SpanIter.ForwardBlock(res, i, j, bs, true);
                }
            });

            return res;
        }

        static public void SmoothMask(bool[,] src, int bs)
        {
            MorphologyR4.Open(src, bs<<1);  // heavily segmentated
            MorphologyR4.Close(src, bs);
        }

        // @ warning: temporary solution
        static public void Padding(bool[,] src, int p)
        {
            int h = src.GetLength(0), w = src.GetLength(1);

            Iterator2D.Forward(0, 0, p, w, (y, x) => src[y, x] = false);            // top
            Iterator2D.Forward(h - p, 0, h, w, (y, x) => src[y, x] = false);        // down
            Iterator2D.Forward(p, 0, h - p, p, (y, x) => src[y, x] = false);        // left
            Iterator2D.Forward(p, w - p, h - p, w, (y, x) => src[y, x] = false);    // right
        }
    }
}
