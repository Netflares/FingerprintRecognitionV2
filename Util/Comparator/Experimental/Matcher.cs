using static System.Math;
using static FingerprintRecognitionV2.Util.Comparator.Experimental.Geometry;

namespace FingerprintRecognitionV2.Util.Comparator.Experimental
{
    static public class Matcher
    {
        static private readonly int MSK = (1 << 16) - 1;

        // probe and candidate
        static public int DebugMatch(Fingerprint probe, Fingerprint candi, ref List<Minutia> res0, ref List<Minutia> res1)
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
            mTriplets.Sort((a, b) => a > b ? -1 : 1);   // descending

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

            // global matching
            int bestMatches = 0;

            foreach (MinutiaPair pair1 in mPairs)
            {
                Minutia m1 = pair1.Probe, m2 = pair1.Candi;
                double theta = m2.A - m1.A;

                int matches = 0;

                List<Minutia> ans0 = new(), ans1 = new();
                ans0.Add(m1); ans1.Add(m2);

                foreach (MinutiaPair pair2 in mPairs)
                {
                    if (pair1.Equals(pair2)) continue;

                    Minutia m3 = pair2.Probe;
                    Minutia m4 = pair2.Candi;

                    // finding p'
                    double x = m2.X + Cos(theta) * (m3.X - m1.X) - Sin(theta) * (m3.Y - m1.Y);
                    double y = m2.Y + Sin(theta) * (m3.X - m1.X) + Cos(theta) * (m3.Y - m1.Y);
                    double t = m3.A - m1.A + m2.A;
                    double edgeA = x - m4.X, edgeB = y - m4.Y, edgeC = edgeA * edgeA + edgeB * edgeB;

                    if (edgeC > Similarity.THRESH_D * Similarity.THRESH_D) continue;
                    if (AdPI(Ad2PI(m2.A, m1.A), Ad2PI(m4.A, m3.A)) > Similarity.THRESH_A) continue;
                    if (AdPI(t, m4.A) > Similarity.THRESH_A) continue;

                    matches++;
                    ans0.Add(m3);
                    ans1.Add(m4);
                }

                if (matches > bestMatches)
                {
                    bestMatches = matches;
                    res0 = ans0;
                    res1 = ans1;
                }
            }

            return bestMatches;
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
