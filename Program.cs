using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Preprocessing;

/*
Image<Gray, byte> src = new("_dat/60.bmp");
ProcImg img = new(src);
CvInvoke.Imwrite("_dat/deb.png", img.Visualize());
*/

bool[,] mat = File2Mat.Read("_dat/o/ske-bin/60.txt", ProcImg.Height, ProcImg.Width);
CvInvoke.Imwrite("_dat/decode.png", MatConverter.Bool2Img(mat));
