namespace FingerprintRecognitionV2.Util.Comparator.Experimental
{
    static public class Matcher
    {
        static private readonly int MSK = (1 << 16) - 1;

        // probe and candidate
        static public int Match(Fingerprint probe, Fingerprint candi)
        {
            List<Triplet> probeT = probe.Triplets, candiT = candi.Triplets;

            // 5.1.2
            List<TripletPair> mTriplets = new();
            foreach (Triplet p in probeT)
            {
                int l = candi.LowerBound(p.Distances[2] - Similarity.THRESH_D);
                if (l == candiT.Count) continue;
                int r = candi.LowerBound(p.Distances[2] + Similarity.THRESH_D);

                for (; l < r; l++)
                {
                    double score = Similarity.sPart(p, candiT[l]);
                    if (score > 0) mTriplets.Add(new(p, candiT[l], score));
                }
            }

            // 5.1.3
            mTriplets.Sort();

            // 5.1.4
            List<MinutiaPair> mPairs = new();

            // 5.1.5
            HashSet<int> probeDupes = new(), candidateDupes = new();
            foreach (TripletPair p in mTriplets)
            {
                for (int i = 0; i < 3; i++)
                {
                    Minutia a = p.Probe.Minutiae[i], b = p.Candi.Minutiae[i];
                    int aKey = Convert.ToInt32(a.Y) << 16 | Convert.ToInt32(a.X),
                        bKey = Convert.ToInt32(b.Y) << 16 | Convert.ToInt32(b.X);
                    if (probeDupes.Add(aKey) || candidateDupes.Add(bKey))
                        mPairs.Add(new(a, b));
                }
            }

            return -1;
        }

        /** 
         * @ containers
         * */
        private class TripletPair
        {
            public Triplet Probe;
            public Triplet Candi;
            public double Score;

            public TripletPair(Triplet probe, Triplet candidate, double score)
            {
                Probe = probe;
                Candi = candidate;
                Score = score;
            }

            static public bool operator <(TripletPair a, TripletPair b) => a.Score < b.Score;

            static public bool operator >(TripletPair a, TripletPair b) => a.Score > b.Score;
        }

        private class MinutiaPair
        {
            public Minutia Probe;
            public Minutia Candi;

            public MinutiaPair(Minutia probe, Minutia candi)
            {
                Probe = probe;
                Candi = candi;
            }
        }
    }
}
