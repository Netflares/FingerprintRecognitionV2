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
        static public void Normalize(Image<Gray, byte> src, double[,] res)
        {
            /*
                `src`:
                    the source image
                    white fg, black bg
                `res`:
                    the img after normalization
                    black fg, white bg
            */

            // std = sqrt( sum((src - avg)**2) / size )
            Gray avgObj;
            MCvScalar stdObj;
            src.AvgSdv(out avgObj, out stdObj);
            double avg = avgObj.Intensity, std = stdObj.V0;

            Iterator2D.Forward(
                res, (y, x) => res[y, x] = NormalizePixel(src[y, x].Intensity, avg, std)
            );
        }

        static private double NormalizePixel(double px, double avg, double std)
        {
            double coeff = Abs(px - avg) / std;
            // flip fg/bg color here
            if (px < avg) 
                return coeff;   // positive --> background
            return -coeff;      // negative --> foreground
        }
    }
}
