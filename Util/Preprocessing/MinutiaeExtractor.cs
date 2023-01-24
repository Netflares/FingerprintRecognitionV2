
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Comparator;
using static System.Math;

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
        static public List<Minutia> Extract(bool[,] ske, bool[,] msk, int bs)
        {
            int h = ske.GetLength(0), w = ske.GetLength(1);
            List<Minutia> res = new();

            Iterator2D.Forward(bs, bs, h - bs, w - bs, (y, x) =>
            {
                if (msk[y, x] && CheckMinutia(ske, y, x) == Minutia.ENDING)
                    HandleEnding(res, ske, y, x, bs);                    
            });
            return res;
        }

        static private void HandleEnding(List<Minutia> res, bool[,] ske, int y, int x, int bs)
        {
            Dictionary<int, List<int>> adj = new();
            List<int> margin = new();
            RidgesExtractor.BFS(ske, y, x, bs, adj, margin);

            // add a noise check here
            if (margin.Count != 1) return;

            int yy = margin[0] >> 8, xx = margin[0] & RidgesExtractor.MSK;
            res.Add(new(Minutia.ENDING, y, x, CalcAlpha(yy, xx, bs, bs)));
        }

        static private byte CheckMinutia(bool[,] ske, int y, int x)
        {
            if (!ske[y, x]) return Minutia.NO_TYPE;
            int n = CountTransitions(ske, y, x);

            if (n == 1) return Minutia.ENDING;
            if (n > 2) return Minutia.BIFUR;
            return Minutia.NO_TYPE;
        }

        static private double CalcAlpha(int y0, int x0, int y1, int x1)
        {
            double dy = y1 - y0, dx = x1 - x0;
            double len = Math.Sqrt(dy * dy + dx * dx);

            double ans = Acos(dx / len);
            return dy >= 0 ? ans : Geometry.V_2PI - ans;
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
