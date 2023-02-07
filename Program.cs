using DelaunatorSharp;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.Util;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.ComparatorSlow;
using FingerprintRecognitionV2.Util.Preprocessing;
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

Fingerprint[] fps = new Fingerprint[500];

for (int i = 0; i < 500; i++)
{
    /*
    ProcImg img = new(new Image<Gray, byte>("_dat/set00/" + i + ".bmp"));
    fps[i] = new(ProcImg.SkeletonMat, ProcImg.SegmentMsk, 16);
    fps[i].Export("_dat/inp-fast-v3/" + i.ToString("D3") + ".inp");
    CvInvoke.Imwrite("_dat/visualize-ver7/" + i.ToString("D3") + ".png", Visualization.Visualize(ProcImg.SkeletonMat, fps[i].Minutiae));
    */
    fps[i] = new("_dat/inp-fast-v3/" + i.ToString("D3") + ".inp");
}

timer.Stop();
PrintTime(timer, "preprocessed 500 fingerprints");

Console.WriteLine("begins to compare 500*499 pairs of images");
timer.Restart();

for (int i = 0; i < 500; i++)
{
    int bestMatches = 0;
    int k = 0;

    List<Minutia> fansProbe = new(), fansCandi = new();

    for (int j = 0; j < 500; j++)
    {
        if (i == j) continue;

        try{
        List<Minutia> ansProbe = new(), ansCandi = new();

        int matches = Matcher.DebugMatch(fps[i], fps[j], ref ansProbe, ref ansCandi);

        if (matches > bestMatches)
        {
            bestMatches = matches;
            k = j;

            fansProbe = ansProbe;
            fansCandi = ansCandi;
        }
        } catch (Exception e) {Console.WriteLine(i + " " + j);}
    }

    Image<Bgr, byte> probe = new("_dat/set00/" + i + ".bmp"), candi = new("_dat/set00/" + k + ".bmp");
    foreach (Minutia a in fansProbe)
        CvInvoke.Circle(probe, new System.Drawing.Point(a.X, a.Y), 4, new(0, 255, 0));
    foreach (Minutia a in fansCandi)
        CvInvoke.Circle(candi, new System.Drawing.Point(a.X, a.Y), 4, new(0, 255, 0));

    Image<Bgr, byte> ans = new(320 * 2, 480);
    Iterator2D.Forward(480, 320, (y, x) => ans[y, x] = probe[y, x]);
    Iterator2D.Forward(480, 320, (y, x) => ans[y, x + 320] = candi[y, x]);

    CvInvoke.Imwrite(
        String.Format("_dat/cmp-v2/{0}-{1}.png", i.ToString("D3"), k.ToString("D3")), 
        ans
    );
}

timer.Stop();
PrintTime(timer, "compared");
