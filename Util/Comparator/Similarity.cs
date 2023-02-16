namespace FingerprintRecognitionV2.Util.Comparator
{
	static public class Similarity
	{
		static public double sPart(Triplet a, Triplet b)
		{
			if (!sTheta(a.Minutiae, b.Minutiae)) return 0;

			double sd = sDistance(a.Distances, b.Distances);
			if (sd >= 1) return 0;

			double sa = sAlpha(CalcAlpha(a.Minutiae), CalcAlpha(b.Minutiae));
			if (sa >= 1) return 0;

			double sb = sBeta(CalcBeta(a.Minutiae), CalcBeta(a.Minutiae));
			if (sb >= 1) return 0;

			return 1 - sd * sa * sb;
		}

		/*
		equation 7
		compare triplets' orientation
		*/
		static private bool sTheta(Minutia[] a, Minutia[] b)
		{
			for (int i = 0; i < 3; i++)
				if (Geometry.AdPI(a[i].Angle, b[i].Angle) >= Param.AngleTolerance) return false;
			return true;
		}

		/*
		equation 8
		compare triplets' length
		*/
		static private double sDistance(double[] a, double[] b)
		{
			double ans = 0;
			for (int i = 0; i < 3; i++)
				ans = Math.Max(ans, Math.Abs(a[i] - b[i]));
			return ans / Param.LocalDistanceTolerance;
		}

		/*
		equation 9

		*/
		static private double sAlpha(double[] a, double[] b)
		{
			double ans = 0;
			for (int i = 0; i < 6; i++)
				ans = Math.Max(ans, Geometry.AdPI(a[i], b[i]));
			return ans / Param.AngleTolerance;
		}

		/*
		equation 10

		*/
		static public double sBeta(double[] a, double[] b)
		{
			double ans = 0;
			for (int i = 0; i < 3; i++)
				ans = Math.Max(ans, Geometry.AdPI(a[i], b[i]));
			return ans / Param.AngleTolerance;
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
                Geometry.Ang(a.Y, a.X, b.Y, b.X), a.Angle
            );
        }

        static public double[] CalcBeta(Minutia[] m)
        {
            return new double[3]
            {
                Geometry.Ad2PI(m[0].Angle, m[1].Angle),
                Geometry.Ad2PI(m[1].Angle, m[2].Angle),
                Geometry.Ad2PI(m[2].Angle, m[0].Angle)
            };
        }
	}
}