using static System.Math;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
    // rotate an image by a trigonometry angle
    /*
    references:
        https://en.wikipedia.org/wiki/Affine_transformation
        https://en.wikipedia.org/wiki/Transformation_matrix#Rotation
        https://www.algorithm-archive.org/contents/affine_transformations/affine_transformations.html
    the idea of the matrix given in wiki is that:
        given a point P, 
        after multiplying it with the matrix M, 
        we got P' which is the point after rotation
    so with every point P(y, x), after transformation it becomes P'(y', x') where:
        y' = x * sin(a) + y * cos(a)
        x' = x * cos(a) - y * sin(a)
    with sin(a) = -sin(-a) and cos(a) = cos(-a), we have the reversed approach:
        y = - x' * sin(a) + y' * cos(a)
        x = + x' * cos(a) + y' * sin(a)
    */
    static public class AffineRotation
    {
        /*
        performs a CLOCKWISE rotatation, not a TRIGONOMETRY rotation

        you can still rotate an image without losing any pixel
        which increase the res image size to:
            [w * cos(rad) + h * sin(rad)] * [w * sin(rad) + h * cos(rad)]

        but I don't think that's worth the performance loss
        */
        static public double[,] KeepSizeRotate(double[,] src, double rad)
        {
            int h = src.GetLength(0), w = src.GetLength(1);
            int cy = h >> 1, cx = w >> 1;
            double ca = Cos(rad), sa = Sin(rad);

            double[,] res = new double[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // given P', calculate P
                    int ry = y - cy, rx = x - cx;               // res y, res x
                    int sy = cy + (int)(-rx * sa + ry * ca),    // src y
                        sx = cx + (int)(+rx * ca + ry * sa);    // src x
                    if (0 <= sy && sy < h && 0 <= sx && sx < w)
                        res[y, x] = src[sy, sx];
                }
            }

            return res;
        }

        /*
        the special affine rotation function, only for the Wavelength Matrix

        src:    the whole image, size = ProcImg.Height * ProcImg.Width
        t, l:   the coord of the block's first pixel
        ss:     the size of the block
        rs:     the size of the result block, this should equal to floor( 16/sqrt(2) )
        rad:    angle to rotate, measured in rad
        */
        static public void NoBackgroundRotate(double[,] src, double[,] res, int t, int l, int ss, int rs, double rad)
        {
            int scy = t + (ss >> 1), scx = l + (ss >> 1);   // src center y coord, src center x coord
            int rcy = rs >> 1, rcx = rs >> 1;               // res center y coord, res center x coord
            double ca = Cos(rad), sa = Sin(rad);

            for (int y = 0; y < rs; y++)
            {
                for (int x = 0; x < rs; x++)
                {
                    int ry = y - rcy, rx = x - rcx;                 // res y, res x
                    int sy = scy + (int)Round(-rx * sa + ry * ca),  // src y
                        sx = scx + (int)Round(+rx * ca + ry * sa);  // src x
                    // be careful
                    res[y, x] = src[sy, sx];
                }
            }
        }
    }
}
