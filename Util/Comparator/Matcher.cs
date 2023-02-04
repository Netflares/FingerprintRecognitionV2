using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.Comparator
{
	static public class Matcher
	{
		static List<TripletPair> mTriplets = new(1000);
		static List<MinutiaPair> mPairs = new(1000);
		static FastHashSet<int> probeDupes = new();
		static FastHashSet<int> candidateDupes = new();

		static public int Match(Fingerprint probe, Fingerprint candidate)
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
					int score = Similarity.sPart(p, candidateT[l]);
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
		}

		/**
		 * @ containers
		 * */
		private class TripletPair
		{
			public Triplet Probe;
			public Triplet Candidate;
			public int Score;

			public TripletPair(Triplet probe, Triplet candidate, int score)
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
	}
}