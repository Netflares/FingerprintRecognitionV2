using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Comparator;
using static System.Math;

namespace FingerprintRecognitionV2.Util.FeaturesExtractor
{
    static public class MinutiaeExtractor
    {
        /** 
         * @ usage:
         * extacts minutiae from a skeleton image
         * 
         * `bool[,] ske`:       a ske image
         * `double[,] orient`:  block orient
         * `int wl`:            ridges' wavelength
         * `int bs`:            block size
         * */
        static public List<Minutia> Extract(bool[,] ske, bool[,] msk, int bs)
        {
            int h = ske.GetLength(0), w = ske.GetLength(1);
            List<Minutia> res = new();

            Iterator2D.Forward(bs, bs, h - bs, w - bs, (y, x) =>
            {
                if (msk[y, x])
                {
                    byte typ = CheckMinutia(ske, y, x);
                    if (typ == Minutia.ENDING)
                        HandleEnding(res, ske, y, x);
                    else if (typ == Minutia.BIFUR)
                        HandleBifur(res, ske, y, x);
                }
            });

            return res;
        }

        static private byte CheckMinutia(bool[,] ske, int y, int x)
        {
            if (!ske[y, x]) return Minutia.NO_TYPE;
            int n = CountTransitions(ske, y, x);

            if (n == 1) return Minutia.ENDING;
            if (n > 2) return Minutia.BIFUR;
            return Minutia.NO_TYPE;
        }

        static private void HandleEnding(List<Minutia> res, bool[,] ske, int y, int x)
        {
            List<double> pts = RidgesExtractor.EndingBFS(ske, y, x, 16, new int[] { 7, 12, 16 });
            if (pts.Count == 0) return; // this is a noise

            // tolerance: 20deg
            if (Abs(pts[1] - pts[0]) <= PI / 9 && Abs(pts[2] - pts[1]) <= PI / 9)
                res.Add(new(y, x, pts[2]));
        }

        static private void HandleBifur(List<Minutia> res, bool[,] ske, int y, int x)
        {
            // get triplets
            List<double[]> trps = RidgesExtractor.BifurBFS(ske, y, x, 16, new int[3] { 3, 10, 16 });
            if (trps.Count == 0) return;
            double[] trp = trps[2];

            double a01 = Geometry.AdPI(trp[0], trp[1]),
                   a02 = Geometry.AdPI(trp[0], trp[2]),
                   a12 = Geometry.AdPI(trp[1], trp[2]);

            double thr = PI / 2;    // threshold = 90deg
            double ang = double.NegativeInfinity;

            if (a12 <= thr) ang = trp[0];
            if (a01 <= thr) ang = trp[2];
            if (a02 <= thr) ang = trp[1];

            if (ang > double.NegativeInfinity)
                res.Add(new(y, x, ang));
        }

        /** 
         * @ dumb tools
         * 
         * I believe this would be faster than a loop
         * */
        static private int CountTransitions(bool[,] ske, int y, int x)
        {
            var p2 = ske[y - 1, x];
            var p3 = ske[y - 1, x + 1];
            var p4 = ske[y, x + 1];
            var p5 = ske[y + 1, x + 1];
            var p6 = ske[y + 1, x];
            var p7 = ske[y + 1, x - 1];
            var p8 = ske[y, x - 1];
            var p9 = ske[y - 1, x - 1];

            return CountTrue(!p2 && p3, !p3 && p4, !p4 && p5, !p5 && p6, !p6 && p7, !p7 && p8, !p8 && p9, !p9 && p2);
        }

        static private int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }
    }
}
