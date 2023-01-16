
using Emgu.CV.Structure;
using Emgu.CV;
using FingerprintRecognitionV2.MatTool;
using static System.Math;

// this class serves debug purposes only
namespace FingerprintRecognitionV2.Util
{
    static public class Visualization
    {
        static public Image<Bgr, byte> Bool2Bgr(bool[,] src)
        {
            int h = src.GetLength(0), w = src.GetLength(1);
            Image<Bgr, byte> res = new(w, h);

            Iterator2D.Forward(1, 1, h - 1, w - 1, (y, x) =>
            {
                int v = src[y, x] ? 255 : 0;
                res[y, x] = new Bgr(v, v, v);
            });
            return res;
        }

        static public void Plot(Image<Bgr, byte> src, int y, int x, int r, Bgr c)
        {
            if (y - r < 0 || src.Height <= y + r || x - r < 0 || src.Width <= x + r)
                return;
            for (int i = y - r; i <= y + r; i++)
                for (int j = x - r; j <= x + r; j++)
                    src[i, j] = c;
        }

        /** 
         * @ params:
         * src: the source img
         * y:   first y loc of line
         * x:   first x loc of line
         * a:   angle
         * l:   length
         * c:   color
         * */
        static public void DrawLine(Image<Bgr, byte> src, int y, int x, double a, int l, int r, Bgr c)
        {
            for (int i = 0; i < l; i++)
                Plot(src, (int)Round(y + i * Sin(a)), (int)Round(x + i * Cos(a)), r, c);
        }
    }
}
