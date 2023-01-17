using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.Util;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Preprocessing;

void BenchMark()
{
    Stopwatch timer = new();
    timer.Start();
    for (int i = 0; i < 500; i++)
    {
        Image<Gray, byte> src = new("_dat/set00/" + i + ".bmp");
        ProcImg img = new(src);
        // CvInvoke.Imwrite("_dat/o/ske/" + i + ".png", MatConverter.Bool2Img(ProcImg.GaborMat));
    }
    timer.Stop();
    ProcImg.PrintTime(timer, "Benchmarked 500 images");
}

void SingleProc()
{
    Image<Gray, byte> src = new("_dat/set00/60.bmp");

    Stopwatch timer = new();
    timer.Start();
    ProcImg img = new(src);
    timer.Stop();
    ProcImg.PrintTime(timer, "proc an image");

    // CvInvoke.Imwrite("_dat/quick-ske.png", MatConverter.Bool2Img(ProcImg.GaborMat));

    Image<Bgr, byte> res = Visualization.Bool2Bgr(ProcImg.GaborMat);
    foreach (Minutiae i in img.Minutiaes)
    {
        Visualization.DrawLine(res, (int)i.Y, (int)i.X, i.A, 12, 0, new Bgr(0, 0, 255));
        Visualization.Plot(res, (int)i.Y, (int)i.X, 1, new Bgr(0, 255, 0));
    }
    CvInvoke.Imwrite("_dat/minutiae.png", res);
}

SingleProc();

Console.WriteLine("Program Executed Sucessfully / Returned Code 0");
