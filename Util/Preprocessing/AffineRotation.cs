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
    }
}
