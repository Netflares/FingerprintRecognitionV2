using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Preprocessing;

namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class RidgesExtractor
    {
        static public int MSK = (1 << 8) - 1;    // 1111 1111

        /*
		params:
			ske[]:	the skeleton image
			y0, x0:	the center location of the region
			r:		the radius of the region
		returns:
			res[dpt][], 
			whose res[d][i] is the i'th ridge's angle at (d + 1) * (r / dpt)
		*/
        static public List<double> EndingBFS(bool[,] ske, int y0, int x0, int r, int[] step)
        {
            bool[,] mat = RegionalBFS(ske, y0, x0, r);
            List<double> res = new(step.Length);

            for (int i = 0; i < step.Length; i++)
            {
                List<int> pts = AngleExtract(mat, r, step[i]);
                if (pts.Count != 1) return new();  // this is a noise
                res.Add(Geometry.Alpha(r, r, pts[0] >> 8, pts[0] & MSK));
            }

            return res;
        }

        static public double[] BifurBFS(bool[,] ske, int y0, int x0, int r)
        {
            bool[,] mat = RegionalBFS(ske, y0, x0, r);

            List<int> ls = AngleExtract(mat, r, r);
            if (ls.Count != 3) return new double[0];

            // clockwise sort
            Minutia[] pts = new Minutia[3];
            for (int i = 0; i < 3; i++)
                pts[i] = new(ls[i] >> 8, ls[i] & MSK);
            Triplet.ShiftClockwise(pts);

            // returns
            double[] res = new double[3];
            for (int i = 0; i < 3; i++)
                res[i] = Geometry.Alpha(r, r, (int)pts[i].Y, (int)pts[i].X);

            return res;
        }

        /*
		Extract the ridge's angle around (c, c)
		This function also merge adj cells
		
		The result is encode as `y<<8|x`
		*/
        static private List<int> AngleExtract(bool[,] mat, int c, int r)
        {
            List<int> res = new();
            bool state = mat[c - r + 1, c - r]; // (top + 1, left)

            // top
            for (int x = -r; x < r; x++)
                AngleExtractTravel(mat, c - r, c + x, c, ref state, res);
            // right
            for (int y = -r; y < r; y++)
                AngleExtractTravel(mat, c + y, c + r, c, ref state, res);
            // down
            for (int x = r; x > -r; x--)
                AngleExtractTravel(mat, c + r, c + x, c, ref state, res);
            // left
            for (int y = r; y > -r; y--)
                AngleExtractTravel(mat, c + y, c - r, c, ref state, res);

            return res;
        }

        static private void AngleExtractTravel(bool[,] mat, int y, int x, int c, ref bool state, List<int> res)
        {
            if (!state && mat[y, x]) res.Add(y << 8 | x);
            state = mat[y, x];
        }

        /*
		perform a DFS around (y0, x0)

		the returning matrix has the size of (2r + 1) ** 2
		whose (r, r) is relative to the initial (y0, x0)
		*/
        static public bool[,] RegionalBFS(bool[,] ske, int y0, int x0, int r)
        {
            y0 -= r; x0 -= r;       // (y0, x0) translates to (r, r)
            int rr = r << 1;

            Deque<int> q = new();
            q.PushBack(r << 8 | r);     // first 8 bits: x; second 8 bits: y;
            bool[,] vst = new bool[r << 1 | 1, r << 1 | 1];
            vst[r, r] = true;

            while (q.Count > 0)
            {
                int id = q.Front(), y = id >> 8, x = id & MSK;
                q.PopFront();

                for (int t = 0; t < 8; t++)
                {
                    int ny = y + MorphologyR8.RY[t], nx = x + MorphologyR8.RX[t];
                    if (ny < 0 || nx < 0 || ny > rr || nx > rr) continue;

                    if (ske[ny + y0, nx + x0] && !vst[ny, nx])
                    {
                        vst[ny, nx] = true;
                        q.PushBack(ny << 8 | nx);
                    }
                }
            }

            return vst;
        }
    }
}
