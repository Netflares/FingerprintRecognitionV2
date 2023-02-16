using DelaunatorSharp;

namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Fingerprint
    {
        public List<Minutia> Minutiae;
        public List<Triplet> Triplets;

        /** 
		 * @ constructors
		 * */
        // create fingerprint from inp data
        public Fingerprint(string fname)
        {
            using FileStream fs = File.OpenRead(fname);
            using StreamReader sr = new(fs);

            Minutiae = new();

            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] items = line.Split(' ');
                Minutiae.Add(new(
                    Convert.ToInt32(items[0]), Convert.ToInt32(items[1]), Convert.ToDouble(items[2])
                ));
            }

            BuildTriplets();
        }

        private void BuildTriplets()
        {
            List<IPoint> pts = new(Minutiae.Count);
            foreach (Minutia m in Minutiae) pts.Add(new Point(m.X, m.Y));
            Delaunator d = new(pts.ToArray());
            int[] t = d.Triangles;

            Triplets = new(t.Length / 3);
            for (int i = 0; i + 2 < t.Length; i += 3)
            {
                Triplets.Add(new Triplet(
                    Minutiae[t[i + 0]], Minutiae[t[i + 1]], Minutiae[t[i + 2]]
                ));
            }

            Triplets.Sort((a, b) => a < b ? -1 : 1);    // check this out later
        }

        /** 
		 * @ util
		 * */
        public int LowerBound(double len)
        {
            int l = 0, r = Triplets.Count;
            while (l < r)
            {
                int m = (l + r + 0) >> 1;
                if (Triplets[m].Distances[2] >= len)
                    r = m;
                else
                    l = m + 1;
            }
            return r;
        }
    }
}
