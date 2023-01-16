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

    List<Minutiae> ls = MinutiaeExtractor.Extract(ProcImg.GaborMat, ProcImg.OrientMat, (int) img.WaveLen, ProcImg.BlockSize);
    Image<Bgr, byte> res = Visualization.Bool2Bgr(ProcImg.GaborMat);

    foreach (Minutiae i in ls) Visualization.DrawLine(res, (int)i.Y, (int)i.X, i.A, 12, 0, new Bgr(0, 0, 255));

    CvInvoke.Imwrite("_dat/minutiae.png", res);
}

SingleProc();

Console.WriteLine("Program Executed Sucessfully / Returned Code 0");
