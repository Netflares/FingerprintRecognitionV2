using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.PreprocessingMultithread
{
    /**
     * @ usage:
     * 
     * apply morphology operations via a dirty circle kernel
     * the input image MUST HAVE THE SAME SIZE as declared in ProcImg
     * */
    public class MorphologyR4
    {
        // used for deque
        private class Position
        {
            public int Y, X, Depth;

            public Position(int y, int x, int d)
            {
                Y = y;
                X = x;
                Depth = d;
            }
        }

        /** 
         * @ settings
         * */
        static public readonly int[] RY = { -1, 0, 1, 0 };
        static public readonly int[] RX = { 0, 1, 0, -1 };
        static readonly int Height = Param.Height, Width = Param.Width, ImgSize = Param.Size;

        /** 
         * @ constructors
         * */
        bool[,] Visited = new bool[Height, Width];

        public MorphologyR4()
        {

        }

        /** 
         * @ core
         * */
        private bool[,] BFS(bool[,] src, bool tar, int bs)
        {
            Deque<Position> q = new();
            Visited = new bool[Height, Width];
            InitDeque(q, src, tar);

            while (q.Count > 0)
            {
                Position cr = q.First();
                q.PopFront();
                if (cr.Depth == bs)
                    continue;

                for (int t = 0; t < 4; t++)
                {
                    int nxtY = cr.Y + RY[t], nxtX = cr.X + RX[t];
                    if (nxtY < 0 || nxtX < 0 || Height <= nxtY || Width <= nxtX)
                        continue;

                    if (src[nxtY, nxtX] != tar && !Visited[nxtY, nxtX])
                    {
                        Visited[nxtY, nxtX] = true;
                        q.PushBack(new(nxtY, nxtX, cr.Depth + 1));
                    }
                }
            }

            return Visited; // returns a reference
        }

        // modifies `src` directly
        unsafe public void Dilate(bool[,] src, int bs)
        {
            bool[,] res = BFS(src, true, bs);   // dilate `1` cells

            Span<bool> srcSpan; fixed (bool* p = src) srcSpan = new(p, ImgSize);
            Span<bool> resSpan; fixed (bool* p = res) resSpan = new(p, ImgSize);

            int i = 0;
            while (i < ImgSize)
                // if (resSpan[i]) srcSpan[i] = true;
                srcSpan[i] |= resSpan[i++];
        }

        // modifies `src` directly
        unsafe public void Erose(bool[,] src, int bs)
        {
            bool[,] res = BFS(src, false, bs);    // dilate `0` cells

            Span<bool> srcSpan; fixed (bool* p = src) srcSpan = new(p, ImgSize);
            Span<bool> resSpan; fixed (bool* p = res) resSpan = new(p, ImgSize);

            int i = 0;
            while (i < ImgSize)
                // if (srcSpan[i] && resSpan[i]) srcSpan[i] = false;
                srcSpan[i] &= !resSpan[i++];
        }

        // modifies `src` directly
        unsafe public void Open(bool[,] src, int bs)
        {
            Erose(src, bs);
            Dilate(src, bs);
        }

        // modifies `src` directly
        unsafe public void Close(bool[,] src, int bs)
        {
            Dilate(src, bs);
            Erose(src, bs);
        }

        /** 
         * @ supporting methods
         * */
        static private void InitDeque(Deque<Position> q, bool[,] src, bool tar)
        {
            Iterator2D.Forward(1, 1, Height - 1, Width - 1, (y, x) =>
            {
                if (src[y, x] != tar) return;
                // assert src[y, x] == tar
                for (int t = 0; t < 4; t++)
                {
                    int nxtY = y + RY[t], nxtX = x + RX[t];
                    if (src[nxtY, nxtX] != tar)
                    {
                        // src[y, x] == margin
                        q.PushBack(new(y, x, 0));
                        return;
                    }
                }
            });
        }
    }
}
