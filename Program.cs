using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util.Preprocessing;

void BenchMark()
{
    Stopwatch timer = new();
    timer.Start();
    for (int i = 0; i < 500; i++)
    {
        Image<Gray, byte> src = new(Constants.DAT_DIR + "set00\\" + i + ".bmp");
        ProcImg img = new(src);
    }
    timer.Stop();
    ProcImg.PrintTime(timer, "Benchmarked 500 images");
}

void SingleProc()
{
    Image<Gray, byte> src = new(Constants.DAT_DIR + "i\\8.bmp");

    Stopwatch timer = new();
    timer.Start();
    ProcImg img = new(src);
    timer.Stop();
    ProcImg.PrintTime(timer, "proc an image");

    CvInvoke.Imwrite("gabor.png", MatConverter.Bool2Img(ProcImg.GaborMat));
}

SingleProc();

Console.WriteLine("Program Executed Sucessfully / Returned Code 0");
