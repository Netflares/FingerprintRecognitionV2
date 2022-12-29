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

        /*
        Iterator2D.Forward(src, (y, x) =>
        {
            if (!img.SegmentMsk[y, x]) src[y, x] = new Gray(50);
            return true;
        });

        CvInvoke.Imwrite(
            String.Format("{0}o\\msk-quick-{1}.png", Constants.DAT_DIR, i), 
            src
        );
        */
    }
    timer.Stop();
    ProcImg.PrintTime(timer, "Benchmarked 500 images");
}

void SingleProc()
{
    Image<Gray, byte> src = new(Constants.DAT_DIR + "i\\0.jpg");
    ProcImg img = new(src);

}

SingleProc();

Console.WriteLine("Program Executed Sucessfully / Returned Code 0");
