
using static System.Math;

namespace FingerprintRecognitionV2.Util.Comparator
{
	static public class Similarity
	{
		/* 
		equation 7
		compare triplets' orientation
		*/
		static public bool sTheta(Minutia[] a, Minutia[] b)
		{
			for (int i = 0; i < 3; i++)
				if (Geometry.AdPI(a[i].A, b[i].A) > PI / 4) return false;
			return true;
		}

		/*
		equation 8
		compare triplets' length
		*/ 
		static public double sDistance(double[] a, double[] b, double thresh)
		{
			double mx = 0;
			for (int i = 0; i < 3; i++)
				mx = Max(mx, Abs(a[i] - b[i]));
			return 1 - mx / thresh;
		}

		/*
		equation 9

		*/
		static public double sAlpha(double[] a, double[] b, double thresh)
		{
			double mx = 0;
			for (int i = 0; i < 6; i++)
				mx = Max(mx, Geometry.AdPI(a[i], b[i]));
			return 1 - mx / thresh;
		}

		/*
		equation 10

		*/
		static public double sBeta(double[] a, double[] b, double thresh)
		{
			double mx = 0;
			for (int i = 0; i < 3; i++)
				mx = Max(mx, Geometry.AdPI(a[i], b[i]));
			return 1 - mx / thresh;
		}
	}
}
