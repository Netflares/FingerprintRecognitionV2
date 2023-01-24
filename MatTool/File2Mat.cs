
namespace FingerprintRecognitionV2.MatTool
{
	static public class File2Mat
	{
		unsafe static public bool[,] Read(string fname, int h, int w)
		{
			int sz = h * w;
			bool[,] res = new bool[h, w];
			Span<bool> arr;
			fixed (bool* p = res) arr = new(p, sz);

			using FileStream fs = File.OpenRead(fname);
            using StreamReader sr = new(fs);
			string buf = sr.ReadLine();

            for (int i = 0; i < sz; i++)
				arr[i] = buf[i] == '0' ? false : true;

			return res;
		}
	}
}
