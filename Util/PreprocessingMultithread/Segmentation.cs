using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.PreprocessingMultithread
{
    public class Segmentation
    {
        static public void CreateMask(double[,] norm, bool[,] res, int bs)
        {
            int height = norm.GetLength(0), width = norm.GetLength(1);
            double bsSqr = bs * bs;

            int mxI = height / bs, mxJ = width / bs;
            Parallel.For(0, mxI, (i) =>
            {
                for (int j = 0; j < mxJ; j++)
                {
                    double avg = MatStatistic.SumBlock(norm, i, j, bs) / bsSqr;
                    double std = MatStatistic.StdBlock(norm, avg, i, j, bs);
                    SpanIter.ForwardBlock(res, i, j, bs, std >= 0.2 || avg < -0.2);
                }
            });
        }

        static public void SmoothMask(bool[,] src, int bs, MorphologyR4 morp)
        {
            morp.Open(src, bs<<1);  // heavily segmentated
            morp.Close(src, bs);
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
