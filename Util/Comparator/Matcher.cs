
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Preprocessing;

namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class Matcher
    {
        // probe and candidate
        static public int Match(ProcImg probe, ProcImg candi)
        {
            List<Triplet> probeT = probe.Triplets, candiT = candi.Triplets;

            // 5.1.2
            List<TripletMatchContainer> mTriplets = new();
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

            return -1;
        }

        /** 
         * @ containers
         * */
        private class TripletMatchContainer
        {
            public Triplet Probe;
            public Triplet Candidate;
            public double Score;

            public TripletMatchContainer(Triplet probe, Triplet candidate, double score)
            {
                Probe = probe;
                Candidate = candidate;
                Score = score;
            }

            static public bool operator <(TripletMatchContainer a, TripletMatchContainer b) => a.Score < b.Score;
        }
    }
}
