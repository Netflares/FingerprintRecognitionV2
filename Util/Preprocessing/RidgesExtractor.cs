
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	static public class RidgesExtractor
	{
		static public int MSK = (1<<8) - 1;    // 1111 1111

		/*
		params:
			ske[]:	the skeleton image
			y0, x0:	the center location of the region
			r:		the radius of the region
		returns:
			res[dpt][], 
			whose res[d][i] is the i'th ridge's angle at (d + 1) * (r / dpt)
		*/
		static public List<List<double>> EndingBFS(bool[,] ske, int y0, int x0, int r, int dpt)
		{
			bool[,] mat = RegionalBFS(ske, y0, x0, r);
			List<List<double>> res = new();
			int step = r / dpt;		// make sure r % dpt == 0

			for (int i = step; i <= r; i += step)
			{
				List<double> angs = AngleExtract(mat, r, i);
				if (angs.Count != 1) return new();	// this is a noise
				res.Add(angs);
			}

			return res;
		}

		static public List<List<double>> BifurBFS(bool[,] ske, int y0, int x0, int r, int dpt)
		{
			
		}

		/*
		Extract the ridge's angle around (c, c)

		This function also merge adj cells
		*/
		static private List<double> AngleExtract(bool[,] mat, int c, int r)
		{
			List<double> res = new();
			bool state = mat[c - r + 1, c - r];	// (top + 1, left)

			// top
			for (int x = -r; x <= r; x++)
				AngleExtractTravel(mat, c - r, x, c, ref state, res);
			// right
			for (int y = -r; y <= r; y++)
				AngleExtractTravel(mat, y, c + r, c, ref state, res);
			// down
			for (int x = r; x >= -r; x--)
				AngleExtractTravel(mat, c + r, x, c, ref state, res);
			// left
			for (int y = r; y >= -r; y--)
				AngleExtractTravel(mat, y, c - r, c, ref state, res);

			return res;
		}

		static private void AngleExtractTravel(bool[,] mat, int y, int x, int c, ref bool state)
		{
			if (!state && mat[y, x]) res.Add(Geometry.Alpha(c, c, y, x));
			state = mat[y, x];
		}

		/*
		perform a DFS around (y0, x0)

		the returning matrix has the size of (2r + 1) ** 2
		whose (r, r) is relative to the initial (y0, x0)
		*/
		static public bool[,] RegionalBFS(bool[,] ske, int y0, int x0, int r)
		{
			y0 -= r; x0 -= r;		// (y0, x0) translates to (r, r)
			int rr = r << 1;

			Deque<int> q = new();
			q.PushBack(r<<8|r);		// first 8 bits: x; second 8 bits: y;
			bool[,] vst = new bool[r<<1|1, r<<1|1];
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
						q.PushBack(ny<<8|nx);
					}
				}
			}

			return vst;
		}
	}
}
