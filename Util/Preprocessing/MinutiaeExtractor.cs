using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Comparator;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class MinutiaeExtractor
    {
        /** 
         * @ usage:
         * extacts minutiae from a skeleton image
         * 
         * @ note:
         * this is still EXPERIMENTAL
         * 
         * `bool[,] ske`:       a ske image
         * `double[,] orient`:  block orient
         * `int wl`:            ridges' wavelength
         * `int bs`:            block size
         * */
        static public List<Minutiae> Extract(bool[,] ske, double[,] orient, bool[,] msk, int wl, int bs)
        {
            int h = ske.GetLength(0), w = ske.GetLength(1);
            List<Minutiae> res = new();
            bool[,] visited = new bool[h, w];

            Iterator2D.Forward(1, 1, h - 1, w - 1, (y, x) =>
            {
                if (msk[y, x] && !visited[y, x])
                {
                    int t = CheckMinutiae(ske, y, x);
                    if (t == Minutiae.NO_TYPE) return;

                    res.Add(new(t, y, x, orient[y / bs, x / bs]));
                    // won't accept more minutiae from this area
                    Iterator2D.Forward(
                        // should have t = y - wl,
                        // but since we won't come back to that area again so...
                        visited, y, x - wl, y + wl, x + wl, (r, c) => visited[r, c] = true
                    );
                }
            });

            return res;
        }

        static private int CheckMinutiae(bool[,] img, int y, int x)
        {
            if (!img[y, x]) return Minutiae.NO_TYPE;
            int n = CountTransitions(img, y, x);

            if (n == 1) return Minutiae.ENDING;
            if (n > 2) return Minutiae.BIFUR;
            return Minutiae.NO_TYPE;
        }

        /** 
         * @ dumb tools
         * 
         * I believe this would be faster than a loop
         * */
        static private int CountTransitions(bool[,] img, int y, int x)
        {
            var p2 = img[y - 1, x];
            var p3 = img[y - 1, x + 1];
            var p4 = img[y, x + 1];
            var p5 = img[y + 1, x + 1];
            var p6 = img[y + 1, x];
            var p7 = img[y + 1, x - 1];
            var p8 = img[y, x - 1];
            var p9 = img[y - 1, x - 1];

            return CountTrue(!p2 && p3, !p3 && p4, !p4 && p5, !p5 && p6, !p6 && p7, !p7 && p8, !p8 && p9, !p9 && p2);
        }

        private static int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }
    }
}
