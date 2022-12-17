using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // `ProcImg`: The Image after pre-processing
    public class ProcImg
    {
        /** 
         * @ public attrs
         * */

        /** 
         * @ private attrs
         * */
        double[,] NormMat;

        /** 
         * @ pipline
         * */
        public ProcImg(Image<Gray, byte> src)
        {
            NormMat = Normalization.Normalize(src, 100, 100);
        }
    }
}
