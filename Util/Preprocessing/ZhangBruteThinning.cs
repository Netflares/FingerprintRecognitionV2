
namespace FingerprintRecognitionV2.Util.Preprocessing
{
	static public class ZhangBruteThinning
	{
        /**
         * @ consts
         * */
        static readonly int Height = Param.Height, Width = Param.Width;
        static readonly int MaxIterations = 7;

        /**
         * @ zhang-suen
         * */
        static public void Thinning(bool[,] src)
        {
            var tmp = (bool[,])src.Clone();
            int cnt = 0;
            int iterations = MaxIterations;
            do  // the missing iteration
            {
                cnt = Step(false, tmp, src);
                tmp = (bool[,])src.Clone();
                cnt += Step(true, tmp, src);
                tmp = (bool[,])src.Clone();
                iterations--;
            }
            while (cnt > 0 && iterations > 0);
            // apply the third rules set
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
         * @ additional skeleton enhancement
         * 
         * the skeleton image built by Zhang-Suen algorithm is, unfortunately,
         * imperfect
         * 
         * thereof, these methods are written
         * to remove unwanted pattern from the skeleton
         * */
        static public bool Clean(bool[,] src, int y, int x) 
        {
            // take p1 - a 1 cell
            // decide whether it would continue be a 1 cell

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
