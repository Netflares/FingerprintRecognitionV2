
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Algorithm
{
    /**
     * @ usage:
     * 
     * apply morphology operations via a dirty circle kernel
     * */
    static public class MorphologyR4
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
        static readonly int[] RY = { -1, 0, 1, 0 };
        static readonly int[] RX = { 0, 1, 0, -1 };

        /** 
         * @ core
         * */
        static private bool[,] BFS(bool[,] src, bool tar, int h, int w, int bs)
        {
            Deque<Position> q = new();
            bool[,] visited = new bool[h, w];
            Init(visited, q, src, tar, h, w);

            while (q.Count > 0)
            {
                Position cr = q.First();
                q.PopFront();
                if (cr.Depth == bs)
                    continue;

                for (int t = 0; t < 4; t++)
                {
                    int nxtY = cr.Y + RY[t], nxtX = cr.X + RX[t];
                    if (nxtY < 0 || nxtX < 0 || h <= nxtY || w <= nxtX)
                        continue;

                    if (src[nxtY, nxtX] != tar && !visited[nxtY, nxtX])
                    {
                        visited[nxtY, nxtX] = true;
                        q.PushBack(new(nxtY, nxtX, cr.Depth + 1));
                    }
                }
            }

            return visited;
        }

        unsafe static public void Dilate(bool[,] src, int bs)
        {
            int h = src.GetLength(0), w = src.GetLength(1);
            int sze = h * w;
            bool[,] res = BFS(src, true, h, w, bs);     // dilate `1` cells

            Span<bool> srcSpan; fixed (bool* p = src) srcSpan = new(p, sze);
            Span<bool> resSpan; fixed (bool* p = res) resSpan = new(p, sze);

            int i = 0;
            while (i < sze)
                // if (resSpan[i]) srcSpan[i] = true;
                srcSpan[i] |= resSpan[i++];
        }

        unsafe static public void Erose(bool[,] src, int bs)
        {
            int h = src.GetLength(0), w = src.GetLength(1);
            int sze = h * w;
            bool[,] res = BFS(src, false, h, w, bs);    // dilate `0` cells

            Span<bool> srcSpan; fixed (bool* p = src) srcSpan = new(p, sze);
            Span<bool> resSpan; fixed (bool* p = res) resSpan = new(p, sze);

            int i = 0;
            while (i < sze)
                // if (srcSpan[i] && resSpan[i]) srcSpan[i] = false;
                srcSpan[i] &= !resSpan[i++];
        }

        unsafe static public void Open(bool[,] src, int bs)
        {
            Erose(src, bs);
            Dilate(src, bs);
        }

        unsafe static public void Close(bool[,] src, int bs)
        {
            Dilate(src, bs);
            Erose(src, bs);
        }

        /** 
         * @ supporting methods
         * */
        static private void Init(bool[,] visited, Deque<Position> q, bool[,] src, bool tar, int h, int w)
        {
            Iterator2D.Forward(1, 1, h - 1, w - 1, (y, x) =>
            {
                if (src[y, x] != tar)
                    return false;
                // assert src[y, x] == tar
                for (int t = 0; t < 4; t++)
                {
                    int nxtY = y + RY[t], nxtX = x + RX[t];
                    if (src[nxtY, nxtX] != tar)
                    {
                        // src[y, x] == margin
                        q.PushBack(new(y, x, 0));
                        return true;
                    }
                }
                return false;
            });
        }
    }
}
