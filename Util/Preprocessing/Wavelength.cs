using FingerprintRecognitionV2.MatTool;
using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // note: not optimized, just works for now

    // this basically is the FrequencyMatrix
    // which is written in documents and the older FingerprintRecognition repos.
    static public class Wavelength
    {
        // block size, block size after rotation
        static readonly int 
            BS = ProcImg.BlockSize, BS2 = BS * BS, 
            KS = (int) Floor(BS / Sqrt(2)), KS2 = KS * KS;
        static readonly int 
            Height = ProcImg.Height, Width = ProcImg.Width;

        // norm size:   Height * Width
        // orient size: (Height / BS) * (Width / BS)
        // msk size:    Height * Width
        static public double GetMedianWavelength(double[,] norm, double[,] orient, bool[,] msk)
        {
            List<double> res = new();
            Iterator2D.Forward(Height / BS, Width / BS, (i, j) =>
            {
                int cnt = MatStatistic.SumBlock(msk, i, j, BS);
                if (cnt == BS2)
                {
                    double wave = Query(norm, i, j, orient[i, j], 5, 15);
                    if (wave > 0) res.Add(wave);
                }
            });
            res.Sort();
            return res[res.Count >> 1]; // if your res.Count == 0, you deserve a crash
        }

        // get ridge wavelength of block (i,j) size=BS*BS
        static private double Query(double[,] norm, int i, int j, double orient, double minWavelen, double maxWavelen)
        {
            double cosOrient = Cos(2.0 * orient);
            double sinOrient = Sin(2.0 * orient);
            double blockOrient = Atan2(sinOrient, cosOrient) / 2.0;

            // rotate the block so that all ridges are horizontal
            double[,] kernel = AffineRotation.NoBackgroundRotate(norm, i * BS, j * BS, BS, KS, -blockOrient);
            double[] ridgeSum = SumProjection(kernel);  // size: KS * KS

            // some statistics
            double avg = 0;
            for (int y = 0; y < KS; y++)
                avg += ridgeSum[y];
            avg /= KS;

            // get dilation and filter out noises
            double[] dilation = Morphology1D.GrayDilation(ridgeSum, 5, 1);
            double[] ridgeNoise = new double[KS];
            for (int y = 0; y < KS; y++)
                ridgeNoise[y] = Abs(dilation[y] - ridgeSum[y]);

            // get peaks
            List<int> peaks = new();
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

        /** 
         * @ calculator
         * */
        // project all points to Oy
        unsafe static private double[] SumProjection(double[,] src)
        {
            double[] res = new double[KS];

            Span<double> arr;
            fixed (double* p = src) arr = new(p, KS2);
            int itr = 0, ind = 0;

            while (itr < KS2)
            {
                Span<double> row = arr.Slice(itr, KS);
                foreach (double v in row)
                    res[ind] += v;
                itr += KS;
                ind++;
            }

            return res;
        }
    }
}
