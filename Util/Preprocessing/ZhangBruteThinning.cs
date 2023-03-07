using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	public class ZhangBruteThinning
	{
        /**
         * @ consts
         * */
        static readonly int Height = Param.Height, Width = Param.Width;
        static readonly int MaxIterations = 7;

        /**
         * @ obj
         * */
        bool[,] tmp = new bool[Height, Width];

        public ZhangBruteThinning() {}

        /**
         * @ zhang-suen
         * */
        public void Thinning(bool[,] src)
        {
            Array.Copy(src, tmp, src.Length);
            int cnt = 0;
            int iterations = MaxIterations;
            do  // the missing iteration
            {
                cnt = Step(false, tmp, src);
                Array.Copy(src, tmp, src.Length);
                cnt += Step(true, tmp, src);
                Array.Copy(src, tmp, src.Length);
                iterations--;
            }
            while (cnt > 0 && iterations > 0);
            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    src[y, x] = src[y, x] && Clean(src, y, x);
        }

        static private int Step(bool step, bool[,] tmp, bool[,] src)
        {
            int cnt = 0;
            Parallel.For(1, Width - 1, x =>
            {
                for (int y = 1; y < Height - 1; y++)
                if (src[y, x] && SuenThinningAlg(x, y, tmp, step))
                {
                    cnt++;
                    src[y, x] = false;
                }
            });
            return cnt;
        }

        static private bool SuenThinningAlg(int x, int y, bool[,] src, bool even)
        {
            var p2 = src[y - 1, x];
            var p3 = src[y - 1, x + 1];
            var p4 = src[y, x + 1];
            var p5 = src[y + 1, x + 1];
            var p6 = src[y + 1, x];
            var p7 = src[y + 1, x - 1];
            var p8 = src[y, x - 1];
            var p9 = src[y - 1, x - 1];

            // calc NumberOfNonZeroNeighbors
            int bp1 = CountTrue(p2, p3, p4, p5, p6, p7, p8, p9);

            if (bp1 >= 2 && bp1 <= 6) // 2nd condition
            {
                // calc NumberOfZeroToOneTransitionFromP9
                int bp2 = CountTrue(
                    !p2 && p3, !p3 && p4, !p4 && p5, !p5 && p6, !p6 && p7, !p7 && p8, !p8 && p9, !p9 && p2
                );

                if (bp2 == 1)
                {
                    if (even)
                        return !(p2 && p4 && p8) && !(p2 && p6 && p8);
                    else
                        return !(p2 && p4 && p6) && !(p4 && p6 && p8);
                }
            }
            return false;
        }

        /**
         * some blocks are not skeletonized after a fixed number of iterations
         * this function is to exclude them out
         * */
        static private void ExcludeBlocks(bool[,] src, bool[,] msk)
        {

        }

        /**
         * the skeleton image built by Zhang-Suen algorithm is, unfortunately,
         * imperfect
         * 
         * thereof, these methods are written
         * to remove unwanted pattern from the skeleton
         * */
        static public bool Clean(bool[,] src, int y, int x) 
        {
            var p2 = src[y - 1, x];
            var p3 = src[y - 1, x + 1];
            var p4 = src[y, x + 1];
            var p5 = src[y + 1, x + 1];
            var p6 = src[y + 1, x];
            var p7 = src[y + 1, x - 1];
            var p8 = src[y, x - 1];
            var p9 = src[y - 1, x - 1];

            if (p9 && p8 && p6 && !p3) return false;
            if (p3 && p4 && p6 && !p9) return false;
            if (p5 && p6 && p8 && !p3) return false;
            if (p4 && p6 && p7 && !p9) return false;
            return true;
        }

        /**
         * @ tools
         * */
        private static int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }
    }
}
