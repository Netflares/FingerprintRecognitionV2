
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
            
        }

        static private byte CheckMinutia(bool[,] ske, int y, int x)
        {
            if (!ske[y, x]) return Minutia.NO_TYPE;
            int n = CountTransitions(ske, y, x);

            if (n == 1) return Minutia.ENDING;
            if (n > 2) return Minutia.BIFUR;
            return Minutia.NO_TYPE;
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
