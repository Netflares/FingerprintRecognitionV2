using DelaunatorSharp;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator.Experimental;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

Fingerprint[] fps = new Fingerprint[500];

for (int i = 0; i < 500; i++)
{
    ProcImg img = new(new Image<Gray, byte>("_dat/set00/" + i + ".bmp"));
    fps[i] = new(ProcImg.SkeletonMat, ProcImg.SegmentMsk, 16);
}

Console.WriteLine("begins to compare 500*499 pairs of images");
Stopwatch timer = new();
timer.Start();

Parallel.For(0, 500, (i) =>
{
    int bestMatches = 0;
    int k = 0;

    List<Minutia> fansProbe = new(), fansCandi = new();

    for (int j = 0; j < 500; j++)
    {
        if (i == j) continue;

        List<Minutia> ansProbe = new(), ansCandi = new();

        int matches = Matcher.DebugMatch(fps[i], fps[j], ref ansProbe, ref ansCandi);

        if (matches > bestMatches)
        {
            bestMatches = matches;
            k = j;

            fansProbe = ansProbe;
            fansCandi = ansCandi;
        }
    }

    if (bestMatches < 4) return;

    Image<Bgr, byte> probe = new("_dat/set00/" + i + ".bmp"), candi = new("_dat/set00/" + k + ".bmp");
    foreach (Minutia a in fansProbe)
        CvInvoke.Circle(probe, new System.Drawing.Point((int)a.X, (int)a.Y), 4, new(0, 255, 0));
    foreach (Minutia a in fansCandi)
        CvInvoke.Circle(candi, new System.Drawing.Point((int)a.X, (int)a.Y), 4, new(0, 255, 0));

    Image<Bgr, byte> ans = new(320 * 2, 480);
    Iterator2D.Forward(480, 320, (y, x) => ans[y, x] = probe[y, x]);
    Iterator2D.Forward(480, 320, (y, x) => ans[y, x + 320] = candi[y, x]);

    CvInvoke.Imwrite(
        String.Format("_dat/cmp/{0}-{1}.png", i.ToString("D3"), k.ToString("D3")), 
        ans
    );
});

timer.Stop();
ProcImg.PrintTime(timer, "compared");
