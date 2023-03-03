using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class LakeRemover
    {
        static private readonly int MSK = (1<<10) - 1;  // 11 1111 1111

        private bool[,] vst = new bool[Param.Height, Param.Width];

        public LakeRemover() {}

        /**
         * @ usage
         * 
         * lakes are not very helpful for the dataset which this program is working on
         * because all the ridges are extremely noisy (like image 353.bmp)
         * 
         * so its best not to use them at all
         * 
         * update: now this function removes small islands as well
         * 
         * @ params
         * 
         * src: The gabor image
         * msk: The segment mask. Make sure the mask has at least 1px padding.
         * lim: If a bg region has its area less than `lim`, it'll be changed to fg
         * */
        public void Exec(bool[,] src, bool[,] msk, int lim)
        {
            vst = new bool[Param.Height, Param.Width];
            for (int y = 1; y < Param.Height - 1; y++)
                for (int x = 1; x < Param.Width - 1; x++)
                    if (msk[y, x] && !vst[y, x]) 
                        BFS(src[y, x], src, msk, vst, y, x, lim);
        }

        private void BFS(bool typ, bool[,] src, bool[,] msk, bool[,] vst, int y0, int x0, int lim)
        {
            List<int> h = new(lim + 1); // history ls
            Deque<int> q = new();
            vst[y0, x0] = true;
            q.PushBack(y0<<10|x0);
            h.Add(q.Back());

            while (q.Count > 0)
            {
                int y = q.Front()>>10, x = q.Front()&MSK;
                q.PopFront();
                for (int t = 0; t < 4; t++)
                {
                    int ny = y + MorphologyR4.RY[t], nx = x + MorphologyR4.RX[t];
                    if (msk[ny, nx] && src[ny, nx] == typ && !vst[ny, nx])
                    {
                        vst[ny, nx] = true;
                        q.PushBack(ny<<10|nx);
                        if (h.Count <= lim) h.Add(q.Back());
                    }
                }
            }

            if (h.Count > lim) return;  // not a lake
            for (int i = 0; i < h.Count; i++)
            {
                int y = h[i]>>10, x = h[i]&MSK;
                src[y, x] = !typ;
            }
        }
    }
}
