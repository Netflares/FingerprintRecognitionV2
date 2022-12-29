
namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // this basically is the FrequencyMatrix
    // which is written in documents and the older FingerprintRecognition repos.
    static public class Wavelength
    {
        static public double GetMedianWavelength()
        {
            return 0.14;    // implement later
        }

        // get the wavelength of block loc=(i, j) size=bs*bs
        static private double Query(double[,] norm, int i, int j, int bs, double orient, int minWavelen, int maxWavelen)
        {
            return 0.14;    // implement later
        }
    }
}
