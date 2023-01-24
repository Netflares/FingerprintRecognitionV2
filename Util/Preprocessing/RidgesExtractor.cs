
using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	static public class RidgesExtractor
	{
		static public int MSK = (1<<8) - 1;    // 1111 1111

		// returns 2 things:
		// - a ridge's tree around (y0, x0)
		// - list of leaves
		static public void BFS(bool[,] ske, int y0, int x0, int r, Dictionary<int, List<int>> adj, List<int> margin)
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

				// found a tile at the margin
				if (y == 0 || x == 0 || y == rr || x == rr) margin.Add(id);

				for (int t = 0; t < 8; t++)
				{
					int ny = y + MorphologyR8.RY[t], nx = x + MorphologyR8.RX[t];
					if (ny < 0 || nx < 0 || ny > rr || nx > rr) continue;

					if (ske[ny + y0, nx + x0] && !vst[ny, nx])
					{
						vst[ny, nx] = true;
						q.PushBack(ny<<8|nx);

						if (!adj.ContainsKey(id))
							adj[id] = new();
						adj[id].Add(ny<<8|nx);
                    }
				}
			}
		}
	}
}
