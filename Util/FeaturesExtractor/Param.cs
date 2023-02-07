namespace FingerprintRecognitionV2.Util.FeaturesExtractor
{
	static public class Param
	{
		static public readonly int
			EndingMinLength = 16,
			BifurMinLength = 10;

		static public readonly int[]
			EndingCheckRadius = new int[] { 7, 12, 16 },
			BifurCheckRadius = new int[] { 3, 5, 10 };
	}
}