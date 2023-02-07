using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.ComparatorSlow
{
	static public class Matcher
	{
		static List<TripletPair> mTriplets = new();
		static List<MinutiaPair> mPairs = new();
		static FastHashSet<int> probeDupes = new();
		static FastHashSet<int> candidateDupes = new();

		/**
		 * @ containers
		 * */
		private class TripletPair
		{
			public Triplet Probe;
			public Triplet Candidate;
			public double Score;

			public TripletPair(Triplet probe, Triplet candidate, double score)
			{
				Probe = probe;
				Candidate = candidate;
				Score = score;
			}
		}

		private class MinutiaPair
		{
			public Minutia Probe;
			public Minutia Candidate;

			public MinutiaPair(Minutia probe, Minutia candidate)
			{
				Probe = probe;
				Candidate = candidate;
			}
		}

		/** 
		 * @ debuggers 
		 * */

		// a cheap copy-pasting, but it works regardless
		static public int DebugMatch(Fingerprint probe, Fingerprint candidate, ref List<Minutia> ansProbe, ref List<Minutia> ansCandidate)
		{
			mTriplets.Clear();
			mPairs.Clear();
			probeDupes.Clear();
			candidateDupes.Clear();

			List<Triplet> probeT = probe.Triplets, candidateT = candidate.Triplets;

			// 5.1.2
			foreach (Triplet p in probeT)
			{
				int l = candidate.LowerBound(p.Distances[2] - Param.LocalDistanceTolerance);
				if (l == candidateT.Count) continue;
				int r = candidate.LowerBound(p.Distances[2] + Param.LocalDistanceTolerance);

				for (; l < r; l++)
				{
					double score = Similarity.sPart(p, candidateT[l]);
					if (score > 0) mTriplets.Add(new(p, candidateT[l], score));
				}
			}

			// 5.1.3
			mTriplets.Sort((a, b) => a.Score > b.Score ? -1 : 1);	// descending

			// 5.1.5
			foreach (TripletPair p in mTriplets)
			{
				for (int i = 0; i < 3; i++)
				{
					Minutia a = p.Probe.Minutiae[i], b = p.Candidate.Minutiae[i];
					int aKey = a.Y << 10 | a.X,
						bKey = b.Y << 10 | b.X;
					if (probeDupes.Add(aKey) || candidateDupes.Add(bKey))
						mPairs.Add(new(a, b));
				}
			}

			// global matching
			int ans = 0;

			foreach (MinutiaPair pair1 in mPairs)
			{
				Minutia m1 = pair1.Probe, m2 = pair1.Candidate;
				
				double theta = m2.Angle - m1.Angle,
					   cosTheta = Math.Cos(theta), 
					   sinTheta = Math.Sin(theta);

				int matches = 0;
				List<Minutia> mProbe = new(), mCandidate = new();
				mProbe.Add(m1);
				mCandidate.Add(m2);

				foreach (MinutiaPair pair2 in mPairs)
				{
					if (pair1.Equals(pair2)) continue;
					Minutia m3 = pair2.Probe, m4 = pair2.Candidate;

					if (Geometry.AdPI(
						Geometry.Ad2PI(m2.Angle, m1.Angle), Geometry.Ad2PI(m4.Angle, m3.Angle)
					) > Param.AngleTolerance) continue;

					// finding p'
					double x = m2.X + cosTheta * (m3.X - m1.X) - sinTheta * (m3.Y - m1.Y),
						   y = m2.Y + sinTheta * (m3.X - m1.X) + cosTheta * (m3.Y - m1.Y),
						   t = m3.Angle - m1.Angle + m2.Angle;
					double edgeA = x - m4.X, edgeB = y - m4.Y, edgeC = edgeA * edgeA + edgeB * edgeB;
					
					if (edgeC > Param.GlobalDistanceTolerance * Param.GlobalDistanceTolerance) continue;
					if (Geometry.AdPI(t, m4.Angle) > Param.AngleTolerance) continue;

					matches++;
					mProbe.Add(m3);
					mCandidate.Add(m4);
				}

				if (matches > ans)
				{
					ans = matches;
					ansProbe = mProbe;
					ansCandidate = mCandidate;
				}
			}

			return ans;
		}
	}
}