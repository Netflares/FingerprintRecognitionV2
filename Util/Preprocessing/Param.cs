namespace FingerprintRecognitionV2.Util.Preprocessing
{
	static public class Param
	{
		// Image Attributes
		static public readonly int 
			Height = 480, Width = 320, Size = Height * Width, 
			BlockSize = 16;

		// Fingerprint Quality Thresholds
		static public readonly double
			STDThreshold = 0.18,
			OCLThreshold = 0.6;

		// Gabor Filter Attributes
		static public readonly double
			AngleInc = Math.PI * 4 / 180;
		static public readonly int 
			GaborMedianFilterRadius = 1,
			GaborMinimumLakeSize = 40;
	}
}