
/**
 * the parameters used in this program is no longer
 * suitable for the dataset mentioned in README.md
 * 
 * it still returns good accuracy (still around 95%)
 * but it focuses on smaller dataset now
 * 
 * I feel like I really need to make an AI module
 * that decides these params automatically tbh
 * */

using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.Util;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.ComparatorSlow;
// using FingerprintRecognitionV2.Util.Preprocessing;
using FingerprintRecognitionV2.Util.PreprocessingMultithread;
using System.Diagnostics;

/** 
 * @ debug tools here 
 * */
static void PrintTime(Stopwatch timer, string m)
{
    Console.WriteLine(string.Format(
        "{0}: {1}:{2}.{3}", m, 
        timer.Elapsed.Minutes, 
        timer.Elapsed.Seconds.ToString("D2"), 
        timer.Elapsed.Milliseconds.ToString("D3")
    ));
}

/** 
 * @ testing here
 * */
Stopwatch timer = new();
timer.Start();

const int threads = 5, imgsPerThread = 500 / threads;

Parallel.For(0, threads, (t) => 
{
    int l = t * imgsPerThread, r = (t + 1) * imgsPerThread;
    Processor proc = new();

    for (int i = l; i < r; i++)
    {
        Image<Gray, byte> src = new("_dat/set00/" + i + ".bmp");
        proc.Process(src);
        CvInvoke.Imwrite("_dat/ske-multithread/" + i + ".png", MatConverter.Bool2Img(proc.SkeletonMat));
    }
});

timer.Stop();
PrintTime(timer, "runtime");
