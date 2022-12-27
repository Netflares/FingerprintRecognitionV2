using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using System.Collections;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class Normalization
    {
        /** 
         * @ First Normalization stage
         * */
        static public void Normalize(Image<Gray, byte> src, double[,] res, double m0, double v0)
        {
            /*
                `src`:
                    the source image
                    white fg, black bg
                `res`:
                    the img after normalization
                    black fg, white bg
                `m0`:
                    avg value of `res`
                `v0`:
                    color span of `res`
            */

            // std = sqrt( sum((src - avg)**2) / size )
            Gray avgObj;
            MCvScalar stdObj;
            src.AvgSdv(out avgObj, out stdObj);
            double avg = avgObj.Intensity, std = stdObj.V0;

            double modifier = Sqrt(v0) / std;

            Iterator2D.Forward(
                res, (y, x) => res[y, x] = NormalizePixel(m0, modifier, src[y, x].Intensity, avg, std)
            );
        }

        /** 
         * @ calculator
         * */
        static private double Sqr(double x) => x * x;

        static private double NormalizePixel(double m0, double modifier, double px, double avg, double std)
        {
            // double coeff = Sqrt( v0 * (px - avg)**2 ) / std
            double coeff = Abs(px - avg) * modifier;
            // flip fg/bg color here
            if (px < std)
                return m0 + coeff;
            return m0 - coeff;
        }
    }
}
