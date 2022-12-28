
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    static public class SobelOperator
    {
        /** 
         * @ static memory
         * */
        static private readonly int Height = ProcImg.Height, Width = ProcImg.Width, Size = ProcImg.ImgSize;

        // Prefix(img[y, x]) = P[y*Width + x]
        static private double[,] P = new double[Height, Width];
        
        /** 
         * @ core
         * */
        unsafe static public void SobelX(double[,] norm, double[,] res)
        {
            for (int y = 1; y < Height - 1; y++)
                for (int x = 0; x < Width; x++)
                    P[y, x] = norm[y - 1, x] + norm[y, x] + norm[y, x] + norm[y + 1, x];

            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    res[y, x] = P[y, x + 1] - P[y, x - 1];
        }

        unsafe static public void SobelY(double[,] norm, double[,] res)
        {
            
        }
    }
}
