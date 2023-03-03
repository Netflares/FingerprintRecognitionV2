using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // clean the gabor image
    // by using a 3x3 median filter
    public class MedianFilter
    {
        private int[,] p = new int[Param.Height, Param.Width];

        public MedianFilter() {}

        /**
         * @ params
         * src: the gabor image
         * k:   kernel size
         * 
         * @ usage
         * reduces the noise of the gabor image
         * with a simple median filter
         * */
        public void Exec(bool[,] src, int k)
        {
            int d = k<<1|1, req = d * d >> 1;
            if (k == 1) 
                SmallK(src, req);
            else
                BigK(src, k, req);
        }

        /**
         * @ tools
         * */
        private void SmallK(bool[,] src, int req)
        {
            // build adj matrix
            Iterator2D.PForward(1, 1, Param.Height - 1, Param.Width - 1, (y, x) =>
            {
                p[y, x] = CountTrue(
                    src[y-1, x-1], src[y-1, x+0], src[y-1, x+1],
                    src[y+0, x-1], src[y+0, x+0], src[y+0, x+1],
                    src[y+1, x-1], src[y+1, x+0], src[y+1, x+1]);
            });
            // upd
            Iterator2D.PForward(1, 1, Param.Height - 1, Param.Width - 1, (y, x) =>
            {
                src[y, x] = src[y, x] && p[y, x] > req;
            });
        }

        private void BigK(bool[,] src, int k, int req)
        {
            // build prefix matrix
            for (int y = 1; y < Param.Height; y++)
                for (int x = 1; x < Param.Width; x++)
                    p[y, x] = p[y-1, x] + p[y, x-1] - p[y-1, x-1] + (src[y, x] ? 1 : 0);
            // upd
            Iterator2D.PForward(k+1, k+1, Param.Height - k, Param.Width - k, (y, x) =>
            {
                int cnt = p[y+k, x+k] - p[y-k-1, x+k] - p[y+k, x-k-1] + p[y-k-1, x-k-1];
                src[y, x] = src[y, x] && cnt > req;
            });
        }

        private static int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }
    }
}