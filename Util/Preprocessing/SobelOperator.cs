
namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class SobelOperator
    {
        /** 
         * @ static memory
         * */
        static private readonly int Height = Param.Height, Width = Param.Width;

        // Prefix(img[y, x]) = P[y*Width + x]
        private double[,] P = new double[Height, Width];

        public SobelOperator()
        {
            
        }
        
        /** 
         * @ core
         * */
        unsafe public void SobelX(double[,] norm, double[,] res)
        {
            for (int y = 1; y < Height - 1; y++)
                for (int x = 0; x < Width; x++)
                    P[y, x] = norm[y - 1, x] + norm[y, x] + norm[y, x] + norm[y + 1, x];

            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    res[y, x] = P[y, x + 1] - P[y, x - 1];
        }

        unsafe public void SobelY(double[,] norm, double[,] res)
        {
            for (int x = 1; x < Width - 1; x++)
                for (int y = 0; y < Height; y++)
                    P[y, x] = norm[y, x - 1] + norm[y, x] + norm[y, x] + norm[y, x + 1];

            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    res[y, x] = P[y + 1, x] - P[y - 1, x];
        }
    }
}
