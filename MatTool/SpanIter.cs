
namespace FingerprintRecognitionV2.MatTool
{
    static public class SpanIter
    {
        /** 
         * @ single matrix
         * */
        unsafe static public void Forward<T>(Span<T> span, int w, int t, int l, int d, int r, T val)
        {
            int col = r - l;        // the width of the block
            int itr = t * w + l;    // the first pointer of this block
            int end = d * w;        // the itr won't go here
            
            while (itr < end)
            {
                Span<T> arr = span.Slice(itr, col);
                foreach (ref T v in arr)
                    v = val;
                itr += w;
            }
        }

        unsafe static public void Forward(bool[,] mat, int t, int l, int d, int r, bool val)
        {
            int h = mat.GetLength(0), w = mat.GetLength(1); // size of the matrix
            int len = h * w;        // size of the matrix
            fixed (bool* p = mat)
            {
                Span<bool> span = new(p, len);
                Forward(span, w, t, l, d, r, val);
            }
        }

        unsafe static public void Forward(bool[,] mat, bool val)
        {
            Forward(mat, 0, 0, mat.GetLength(0), mat.GetLength(1), val);
        }

        unsafe static public void ForwardBlock(bool[,] mat, int y, int x, int bs, bool val)
        {
            int t = y * bs, l = x * bs;
            Forward(mat, t, l, t + bs, l + bs, val);
        }
    }
}
