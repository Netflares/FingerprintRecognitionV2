using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Preprocessing;

Image<Gray, byte> src = new(Constants.DAT_DIR + "i\\0.jpg");
ProcImg img = new(src);

CvInvoke.Imwrite(Constants.DAT_DIR + "i\\0-mask-smooth.png", MatConverter.Bool2Disp(img.SegmentMsk));

Console.WriteLine("Program Executed Sucessfully / Returned Code 0");
