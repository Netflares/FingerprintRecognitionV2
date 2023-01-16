
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Comparator;

/** @ WARNING: Experimental class */
namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class MinutiaeExtractor
    {
        /** 
         * @ usage:
         * extacts minutiae from a skeletonized gabor image
         * 
         * @ note:
         * this is still EXPERIMENTAL
         * 
         * `bool[,] gabor`:     a skeletonized gabor image
         * `double[,] orient`:  block orient
         * `int wl`:            ridges' wavelength
         * `int bs`:            block size
         * */
        static public List<Minutiae> Extract(bool[,] gabor, double[,] orient, int wl, int bs)
        {
            int h = gabor.GetLength(0), w = gabor.GetLength(1);
            List<Minutiae> res = new();
            bool[,] msk = new bool[h, w];

            Iterator2D.Forward(1, 1, h - 1, w - 1, (y, x) =>
            {
                if (!msk[y, x] && IsMinutiae(gabor, y, x))
                {
                    res.Add(new(y, x, orient[y / bs, x / bs]));
                    // won't accept more minutiae from this area
                    Iterator2D.Forward(
                        // should have t = y - wl,
                        // but since we won't come back to that area again so...
                        msk, y, x - wl, y + wl, x + wl, (r, c) => msk[r, c] = true
                    );
                }
            });

            return res;
        }

        static private bool IsMinutiae(bool[,] img, int y, int x)
        {
            if (!img[y, x]) return false;
            int n = CountTransitions(img, y, x);
            return n != 0 && n != 2;
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
