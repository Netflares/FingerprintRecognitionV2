using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // this basically is the FrequencyMatrix
    // which is written in documents and the older FingerprintRecognition repos.
    public class Wavelength
    {
        // block size, block size after rotation
        static readonly int 
            BS = Param.BlockSize, BS2 = BS * BS, 
            KS = (int) Floor(BS / Sqrt(2)), KS2 = KS * KS;
        static readonly int 
            Height = Param.Height, Width = Param.Width;
        static readonly int
            Area = (Height / BS) * (Width / BS);

        /**
         * @ params:
         * norm size:   Height * Width
         * orient size: (Height / BS) * (Width / BS)
         * msk size:    Height * Width
         * */

        List<double> waves = new(Area);

        public double GetMedianWavelength(double[,] norm, double[,] orient, bool[,] msk)
        {
            waves.Clear();
            Iterator2D.Forward(Height / BS, Width / BS, (i, j) =>
            {
                int cnt = MatStatistic.SumBlock(msk, i, j, BS);
                if (cnt < BS2) return;
                // if the whole block is within the mask
                double wave = Query(norm, i, j, orient[i, j], 5, 15);
                if (wave > 0) waves.Add(wave);
            });
            if (waves.Count() == 0) return -1;
            waves.Sort();
            return waves[waves.Count >> 1]; // if your waves.Count == 0, you deserve a crash
        }

        /** 
         * @ some really ugly stuffs
         * */
        // pre-allocate memory zones because I don't trust C#
        double[,] kernel = new double[KS, KS];
        double[] ridgeSum = new double[KS];
        double[] dilation = new double[KS];
        double[] ridgeNoise = new double[KS];
        List<int> peaks = new(KS);

        // get ridge wavelength of block (i,j) size=BS*BS
        private double Query(double[,] norm, int i, int j, double orient, double minWavelen, double maxWavelen)
        {
            double cosOrient = Cos(2.0 * orient);
            double sinOrient = Sin(2.0 * orient);
            double blockOrient = Atan2(sinOrient, cosOrient) / 2.0;

            // rotate the block so that all ridges are horizontal
            AffineRotation.NoBackgroundRotate(norm, kernel, i * BS, j * BS, BS, KS, -blockOrient);  // this renews `kernel`
            SumProjection(kernel, ridgeSum);  // size: KS * KS, this renews `ridgeSum`

            // some statistics
            double avg = 0;
            for (int y = 0; y < KS; y++)
                avg += ridgeSum[y];
            avg /= KS;

            // get dilation and filter out noises
            Morphology1D.GrayDilation(ridgeSum, dilation, 5, 1);    // this renews `dilation`
            for (int y = 0; y < KS; y++)
                ridgeNoise[y] = Abs(dilation[y] - ridgeSum[y]);     // this renews `ridgeNoise`

            // get peaks
            peaks.Clear();
            for (int y = 0; y < KS; y++)
                if (ridgeNoise[y] < 2 && ridgeSum[y] > avg)
                    peaks.Add(y);

            // Determine the spatial frequency of the ridges by dividing the
            // distance between the 1st and last peaks by the (No of peaks-1). If no
            // peaks are detected, or the wavelength is outside the allowed bounds, the frequency image is set to 0
            if (peaks.Count < 2)
                return 0.0;
            double waveLength = (double)(peaks.Last() - peaks.First()) / (peaks.Count - 1);
            if (minWavelen <= waveLength && waveLength <= maxWavelen)
                return waveLength;
            return 0.0;
        }

        public Wavelength() {}

        /** 
         * @ calculator
         * */
        // project all points to Oy
        unsafe static private void SumProjection(double[,] src, double[] res)
        {
            Span<double> arr;
            fixed (double* p = src) arr = new(p, KS2);
            int itr = 0, ind = 0;

            while (itr < KS2)
            {
                res[ind] = 0;   // refresh the result array

                Span<double> row = arr.Slice(itr, KS);
                foreach (double v in row)
                    res[ind] += v;
                itr += KS;
                ind++;
            }
        }
    }
}
