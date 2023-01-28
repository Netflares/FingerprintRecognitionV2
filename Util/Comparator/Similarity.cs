
using static System.Math;

namespace FingerprintRecognitionV2.Util.Comparator
{
	static public class Similarity
	{
		static public readonly double THRESH_D = 20.0;
		static public readonly double THRESH_A = PI / 6;

		/*
		equation 6

		*/
		static public double sPart(Minutia[] a, Minutia[] b)
		{
			if (!sTheta(a, b)) return 0;
			double sd = sDistance(Triplet.CalcSortedDist(a), Triplet.CalcSortedDist(b), THRESH_D),
			       sa = sAlpha(CalcAlpha(a), CalcAlpha(b), THRESH_A),
			       sb = sBeta(CalcBeta(a), CalcBeta(b), THRESH_A);
			if (sd == 0 || sa == 0 || sb == 0)
				return 0;
			return 1 - (1 - sd) * (1 - sa) * (1 - sb);
		}

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
			return mx >= thresh ? 0 : 1 - mx / thresh;
		}

		/*
		equation 9

		*/
		static public double sAlpha(double[] a, double[] b, double thresh)
		{
			double mx = 0;
			for (int i = 0; i < 6; i++)
				mx = Max(mx, Geometry.AdPI(a[i], b[i]));
			return mx >= thresh ? 0 : 1 - mx / thresh;
		}

		/*
		equation 10

		*/
		static public double sBeta(double[] a, double[] b, double thresh)
		{
			double mx = 0;
			for (int i = 0; i < 3; i++)
				mx = Max(mx, Geometry.AdPI(a[i], b[i]));
			return mx >= thresh ? 0 : 1 - mx / thresh;
		}

		/*
		ang(p, q), theta
		*/
		static public double[] CalcAlpha(Minutia[] m)
		{
			return new double[6]
			{
				CalcAlpha(m[0], m[1]),
				CalcAlpha(m[0], m[2]),
				CalcAlpha(m[1], m[0]),
				CalcAlpha(m[1], m[2]),
				CalcAlpha(m[2], m[0]),
				CalcAlpha(m[2], m[1]),
			};
		}

		static public double CalcAlpha(Minutia a, Minutia b)
		{
			return Geometry.Ad2PI(
				Geometry.Ang(a.Y, a.X, b.Y, b.X), a.A
			);
		}

		static public double[] CalcBeta(Minutia[] m)
		{
			return new double[3]
			{
				Geometry.Ad2PI(m[0].A, m[1].A),
				Geometry.Ad2PI(m[1].A, m[2].A),
				Geometry.Ad2PI(m[2].A, m[0].A)
			};
		}
	}
}
