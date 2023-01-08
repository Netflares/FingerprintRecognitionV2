
using System.Runtime.InteropServices;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Skeletonization
    {
        static readonly int Height = ProcImg.Height, Width = ProcImg.Width;
        static readonly int MaxIterations = 7;

        static readonly int Mask16 = (1 << 16) - 1;

        static public void Thinning(bool[,] src)
        {
            // the location of white cells
            // the 16 bits on the left are `x` loc
            // the 16 bits on the right are `y` loc
            HashSet<int> whites = FlattenWhiteCells(src);
            List<int> dumpster = new();

            int itr = 0, cnt = 0;
            do
            {
                cnt = Step(false, src, whites, dumpster);
                Update(src, dumpster);
                cnt += Step(true, src, whites, dumpster);
                Update(src, dumpster);
            } while (cnt > 0 && ++itr < MaxIterations);
        }

        static private int Step(bool step, bool[,] src, HashSet<int> whites, List<int> dumpster)
        {
            foreach (int i in whites)
            {
                int y = (i >> 16) & Mask16, x = i & Mask16;
                if (SuenThinningAlg(y, x, src, step))
                {
                    whites.Remove(i);
                    dumpster.Add(i);
                }
            }
            return dumpster.Count;
        }

        // update the `src` mat after each step
        // this will clear the dumpster
        unsafe static private void Update(bool[,] src, List<int> dumpster)
        {
            Span<int> arr = CollectionsMarshal.AsSpan(dumpster);
            foreach (int i in arr)
                src[(i >> 16) & Mask16, i & Mask16] = false;
            dumpster.Clear();
        }

        static private bool SuenThinningAlg(int y, int x, bool[,] src, bool even)
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
         * @ utils
         * */
        unsafe static private HashSet<int> FlattenWhiteCells(bool[,] src)
        {
            HashSet<int> res = new();

            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    if (src[y, x]) res.Add((y<<16) | x);

            return res;
        }

        private static int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }
    }
}
