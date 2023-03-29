using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	/**
	 * @ usage:
	 * 
	 * apply morphology operations via a dirty circle kernel
	 * the input image MUST HAVE THE SAME SIZE as declared in ProcImg
	 * */
	public class MorphologyR4
	{
		// used for deque
		private class Position
		{
			public int Y, X, Depth;

			public Position(int y, int x, int d)
			{
				Y = y;
				X = x;
				Depth = d;
			}
		}

		/** 
		 * @ image properties
		 * */
		static readonly int Height = Param.Height, Width = Param.Width, ImgSize = Param.Size;

		/**
		 * @ relative cells settings
		 * */
		static public readonly int[] RY = { -1, 0, 1, 0 };
		static public readonly int[] RX = { 0, 1, 0, -1 };
		static public readonly int RC = 4;

		protected virtual int GetRY(int t) => RY[t];
		protected virtual int GetRX(int t) => RX[t];
		protected virtual int GetRC() => RC;

		/**
		 * @ this
		 * */
		bool[,] vst = new bool[Height + Param.MorphologyPadding, Width + Param.MorphologyPadding];

		public MorphologyR4() {}

		/**
		 * @ simple methods
		 * */
		public void Erose(bool[,] src, int dep) 
		{
			BFS(src, false, dep);	// dilates `0` cells
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++)
					src[y, x] &= !vst[y, x];
		}

		public void Dilate(bool[,] src, int dep)
		{
			BFS(src, true, dep);	// dilates `0` cells
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++)
					src[y, x] |= vst[y, x];
		}

		// dilates `tar` cells
		private void BFS(bool[,] src, bool tar, int dep) 
		{
			Array.Clear(vst, 0, vst.Length);
			Deque<Position> q = InitDeque(src, tar);
			while (q.Count > 0)
			{
				Position cr = q.Front();
				q.PopFront();
				for (int t = 0; t < GetRC(); t++) 
				{
					int ny = cr.Y + GetRY(t), nx = cr.X + GetRX(t);
					if (
						0 <= ny && ny < Height && 0 <= nx && nx < Width &&
						src[ny, nx] != tar &&
						!vst[ny, nx]
					) {
						vst[ny, nx] = true;
						if (cr.Depth < dep) q.PushBack(new(ny, nx, cr.Depth + 1));
					}
				}
			}
		}

		/**
		 * @ open & close
		 * */
		public void Open(bool[,] src, int dep)
		{
			Array.Clear(vst, 0, vst.Length);
			BurnToVst(src, dep);
			VstBFS(false, dep, dep);	// erose
			VstBFS(true, dep, dep);		// dilate

			Iterator2D.Forward(src, (y, x) => src[y, x] = vst[y + dep, x + dep]);
		}

		// dilate -> erose
		public void Close(bool[,] src, int dep)
		{
			Array.Clear(vst, 0, vst.Length);
			BurnToVst(src, dep);
			VstBFS(true, dep, dep);		// dilate
			VstBFS(false, dep, dep);	// erose

			Iterator2D.Forward(src, (y, x) => src[y, x] = vst[y + dep, x + dep]);
		}

		private void VstBFS(bool tar, int dep, int pad)
		{
			int h = Height + (pad<<1), w = Width + (pad<<1);
			Deque<Position> q = InitDeque(vst, tar);

			while (q.Count > 0) 
			{
				Position cr = q.Front();
				q.PopFront();
				for (int t = 0; t < GetRC(); t++)
				{
					int ny = cr.Y + GetRY(t), nx = cr.X + GetRX(t);
					if (
						0 <= ny && ny < h && 0 <= nx && nx < w &&
						vst[ny, nx] != tar
					) {
						vst[ny, nx] = tar;
						if (cr.Depth < dep) q.PushBack(new(ny, nx, cr.Depth + 1));
					}
				}
			}
		}

		/**
		 * @ utils
		 * */
		private Deque<Position> InitDeque(bool[,] src, bool tar) 
		{
			int h = src.GetLength(0), w = src.GetLength(1);
			Deque<Position> q = new();

			Iterator2D.Forward(src, (y, x) => 
			{
				if (src[y, x] != tar) 
					return;	// not the one we want to dilate

				for (int t = 0; t < GetRC(); t++) {
					int ny = y + GetRY(t), nx = x + GetRX(t);
					if (ny < 0 || nx < 0 || ny >= h || nx >= w) continue;
					// src[y, x] is adjacent to a cell of different color
					if (src[ny, nx] != tar) 
					{
						q.PushBack(new(y, x, 0));
						return;
					}
				}
			});
			return q;
		}

		private void BurnToVst(bool[,] src, int p) 
		{
			Iterator2D.Forward(src, (y, x) => vst[y + p, x + p] = src[y, x]);
		}
	}
}
