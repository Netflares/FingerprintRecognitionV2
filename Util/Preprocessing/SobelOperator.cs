
namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class SobelOperator
    {
        /** 
         * @ static memory
         * */
        static private readonly int Height = ProcImg.Height, Width = ProcImg.Width, Size = ProcImg.ImgSize;

        // Prefix(img[y, x]) = P[y*Width + x]
        static private double[] _P = new double[Size];
        
        /** 
         * @ core
         * */
        unsafe static public void SobelX(double[,] norm, double[,] res)
        {
            // get prefix array
            Span<double> p; 
            fixed (double* ptr = _P) p = new(ptr, Size);

        }

        unsafe static public void SobelY(double[,] norm, double[,] res)
        {
            // get prefix array
            Span<double> p;
            fixed (double* ptr = _P) p = new(ptr, Size);

        }
    }
}
