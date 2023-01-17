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
            bool[,] vst = new bool[h, w];
            byte[,] mat = new byte[h, w];

            Iterator2D.Forward(1, 1, h - 1, w - 1, (y, x) =>
            {
                if (msk[y, x] && !vst[y, x])
                {
                    byte t = CheckMinutiae(ske, y, x);
                    if (t == Minutiae.NO_TYPE) return;

                    mat[y, x] = t;
                    // won't accept more minutiae from this area
                    Iterator2D.Forward(
                        // should have t = y - wl,
                        // but since we won't come back to that area again so...
                        vst, y, x - wl, y + wl, x + wl, (r, c) => vst[r, c] = true
                    );
                }
            });

            ClearNoise(mat, h, w, wl);
            List<Minutiae> res = new();

            Iterator2D.Forward(mat, (y, x) =>
            {
                if (mat[y, x] != Minutiae.NO_TYPE) res.Add(
                    new(mat[y, x], y, x, orient[y / bs, x / bs])
                );
            });

            return res;
        }

        static private byte CheckMinutiae(bool[,] img, int y, int x)
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

        static private int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }

        /** 
         * @ usage:
         * clear noises
         * */
        static private void ClearNoise(byte[,] src, int h, int w, int wl)
        {
            bool[,] msk = new bool[h, w];
            int[,] pre = CreatePrefix(src, h, w);

            Iterator2D.Forward(wl, wl, h - wl, w - wl, (y, x) =>
            {

            });

            MorphologyR4.Close(msk, 8);
            Iterator2D.PForward(msk, (y, x) =>
            {
                if (msk[y, x]) pre[y, x] = Minutiae.NO_TYPE;
            });
        }

        /**
         * @ result:
         * res[y, x] = sum(src[0:y, 0:x])
         * 
         * @ note:
         * this method is optimized for this class only
         * to use this for general purposes, uncomment some lines below
         * */
        static private int[,] CreatePrefix(byte[,] src, int h, int w)
        {
            int[,] res = new int[h, w];

            // // can skip this because there's no minutiae in this edge
            // for (int y = 0; y < h; y++) if (src[y, 0] != Minutiae.NO_TYPE) res[y, 0] = 1;
            // for (int x = 1; x < w; x++) if (src[0, x] != Minutiae.NO_TYPE) res[0, x] = 1;

            for (int y = 1; y < h; y++)
            {
                for (int x = 1; x < w; x++)
                {
                    res[y, x] = res[y - 1, x] + res[y, x - 1];
                    if (src[y, x] != Minutiae.NO_TYPE) res[y, x]++;
                }
            }

            return res;
        }
    }
}
