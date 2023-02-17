
using Emgu.CV.Structure;
using Emgu.CV;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Comparator;
using System.Drawing;
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

        static public Image<Bgr, byte> VisualizeImage(bool[,] ske, List<Minutia> minutiae)
        {
            Image<Bgr, byte> res = Bool2Bgr(ske);

            foreach (var i in minutiae)
            {
                CvInvoke.Circle(res, new Point(i.X, i.Y), 4, new(0, 255, 0));
                Point desPt = new(
                    Convert.ToInt32(i.X + 12 * Cos(i.Angle)), 
                    Convert.ToInt32(i.Y + 12 * Sin(i.Angle))
                );
                CvInvoke.Line(res, new Point(i.X, i.Y), desPt, new(0, 0, 255));
            }
            return res;
        }

        static public Image<Bgr, byte> VisualizeComparison(string fProbe, string fCandi, int h, int w, List<Minutia> mProbe, List<Minutia> mCandi)
        {
            Image<Bgr, byte> probe = new(fProbe), candi = new(fCandi);
            foreach (Minutia m in mProbe)
                CvInvoke.Circle(probe, new Point(m.X, m.Y), 4, new(0, 255, 0));
            foreach (Minutia m in mCandi)
                CvInvoke.Circle(candi, new Point(m.X, m.Y), 4, new(0, 255, 0));

            Image<Bgr, byte> ans = new(w * 2, h);
            Iterator2D.Forward(h, w, (y, x) => ans[y, x] = probe[y, x]);
            Iterator2D.Forward(h, w, (y, x) => ans[y, x + w] = candi[y, x]);

            return ans;
        }
    }
}
