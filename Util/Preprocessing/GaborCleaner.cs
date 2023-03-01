using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // clean the gabor image
    // by using a 3x3 median filter
    public class GaborCleaner
    {
        private int[,] p = new int[Param.Height, Param.Width];

        public GaborCleaner() {}

        /**
         * @ params
         * 
         * src: the gabor image
         * k:   kernel size
         * */
        public void Clean(bool[,] src, int k)
        {
            Build(src);
            int d = k<<1|1, req = d * d >> 1;

            Iterator2D.PForward(k+1, k+1, Param.Height - k, Param.Width - k, (y, x) =>
            {
                int cnt = p[y+k, x+k] - p[y-k-1, x+k] - p[y+k, x-k-1] + p[y-k-1, x-k-1];
                src[y, x] = cnt > req;
            });
        }

        private void Build(bool[,] src)
        {
            for (int y = 1; y < Param.Height; y++)
                for (int x = 1; x < Param.Width; x++)
                    p[y, x] = p[y-1, x] + p[y, x-1] - p[y-1, x-1] + (src[y, x] ? 1 : 0);
        }
    }
}