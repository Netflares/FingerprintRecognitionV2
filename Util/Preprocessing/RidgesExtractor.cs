
using FingerprintRecognitionV2.DataStructure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	static public class RidgesExtractor
	{
		static private int MSK = (1<<8) - 1;	// 11111111

		// returns a ridge's tree around (y0, x0)
		static public Dictionary<int, List<int>> BFS(bool[,] ske, int y0, int x0, int r) 
		{
			Dictionary<int, List<int>> adj = new();

			y0 -= r; x0 -= r;		// (y0, x0) translates to (r, r)
			int rr = r << 1;

			Deque<int> q = new();
			q.PushBack(r<<8|r);		// first 8 bits: x; second 8 bits: y;
			bool[,] vst = new bool[r<<1|1, r<<1|1];

			while (q.Count > 0)
			{
				int id = q.Front(), y = id >> 8, x = id & MSK;
				q.PopFront();

				for (int t = 0; t < 4; t++)
				{
					int ny = y + MorphologyR4.RY[t], nx = x + MorphologyR4.RX[t];
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

			return adj;
		}
	}
}
