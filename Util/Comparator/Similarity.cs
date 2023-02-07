namespace FingerprintRecognitionV2.Util.Comparator
{
	static public class Similarity
	{
		/*
		equation 6
		compare 2 triplets

		return range: 
			[0: Tolerance]
		where:
			`0` means completely mismatched
			`Tolerance` means completely matched

		note:
			the returning statement is modified

		original:
			1 - (1 - sd)(1 - sa)(1 - sb)

		let:
			sd' = (1 - sd) * LocalDistanceTolerance
			sa' = (1 - sa) * AngleTolerance
			sb' = (1 - sb) * AngleTolerance
			Tolerance = LocalDistanceTolerance * AngleTolerance * AngleTolerance
		which are equivalent to:
			sd' = Max{ Abs(d_t[i] - d_r[i]) }
			sa' = Max{ Abs(a_t[i] - a_r[i]) }
			sb' = Max{ Abs(b_t[i] - b_r[i]) }

		the original equation transforms to:
			1 - (sd' * sa' * sb') / Tolerance

		and thus, the returning statement is modified to:
			Tolerance - (sd' * sa' * sb')
		*/
		static public int sPart(Triplet a, Triplet b)
		{
			if (!sTheta(a.Minutiae, b.Minutiae)) return 0;

			int sd = sDistance(a.Distances, b.Distances);
			if (sd >= Param.LocalDistanceTolerance) return 0;

			int sa = sAlpha(CalcAlpha(a.Minutiae), CalcAlpha(b.Minutiae));
			if (sa >= Param.AngleTolerance) return 0;

			int sb = sBeta(CalcBeta(a.Minutiae), CalcBeta(a.Minutiae));
			if (sb >= Param.AngleTolerance) return 0;

			return Param.ToleranceProduct - sd * sa * sb;
		}

		/*
		equation 7
		compare triplets' orientation
		*/
		static private bool sTheta(Minutia[] a, Minutia[] b)
		{
			for (int i = 0; i < 3; i++)
				if (FastGeometry.AdPI(a[i].Angle, b[i].Angle) >= Param.AngleTolerance) return false;
			return true;
		}

		/*
		equation 8
		compare triplets' length
		*/
		static private int sDistance(int[] a, int[] b)
		{
			int ans = 0;
			for (int i = 0; i < 3; i++)
				ans = Math.Max(ans, Math.Abs(a[i] - b[i]));
			return ans;
		}

		/*
		equation 9

		*/
		static private int sAlpha(int[] a, int[] b)
		{
			int ans = 0;
			for (int i = 0; i < 6; i++)
				ans = Math.Max(ans, FastGeometry.AdPI(a[i], b[i]));
			return ans;
		}

		/*
		equation 10

		*/
		static public int sBeta(int[] a, int[] b)
		{
			int ans = 0;
			for (int i = 0; i < 3; i++)
				ans = Math.Max(ans, FastGeometry.AdPI(a[i], b[i]));
			return ans;
		}

		/*
		ang(p, q), theta
		*/
        static public int[] CalcAlpha(Minutia[] m)
        {
            return new int[6]
            {
                CalcAlpha(m[0], m[1]),
                CalcAlpha(m[0], m[2]),
                CalcAlpha(m[1], m[0]),
                CalcAlpha(m[1], m[2]),
                CalcAlpha(m[2], m[0]),
                CalcAlpha(m[2], m[1]),
            };
        }

        static public int CalcAlpha(Minutia a, Minutia b)
        {
            return FastGeometry.Ad2PI(
                FastGeometry.Ang(a.Y, a.X, b.Y, b.X), a.Angle
            );
        }

        static public int[] CalcBeta(Minutia[] m)
        {
            return new int[3]
            {
                FastGeometry.Ad2PI(m[0].Angle, m[1].Angle),
                FastGeometry.Ad2PI(m[1].Angle, m[2].Angle),
                FastGeometry.Ad2PI(m[2].Angle, m[0].Angle)
            };
        }
	}
}