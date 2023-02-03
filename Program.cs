using DelaunatorSharp;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator.Experimental;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

ProcImg img0 = new(new Image<Gray, byte>("_dat/set00/0.bmp"));
Fingerprint probe = new(ProcImg.SkeletonMat, ProcImg.SegmentMsk, 16);
ProcImg img1 = new(new Image<Gray, byte>("_dat/set00/1.bmp"));
Fingerprint candi = new(ProcImg.SkeletonMat, ProcImg.SegmentMsk, 16);

List<Minutia> ansProbe = new(), ansCandi = new();

int matches = Matcher.DebugMatch(probe, candi, ref ansProbe, ref ansCandi);
Console.WriteLine(matches);

Image<Bgr, byte> deb0 = new("_dat/set00/0.bmp"), deb1 = new("_dat/set00/1.bmp");
foreach (Minutia a in ansProbe)
    CvInvoke.Circle(deb0, new System.Drawing.Point((int)a.X, (int)a.Y), 4, new(0, 255, 0));
foreach (Minutia a in ansCandi)
    CvInvoke.Circle(deb1, new System.Drawing.Point((int)a.X, (int)a.Y), 4, new(0, 255, 0));

CvInvoke.Imwrite("_dat/deb/probe.png", deb0);
CvInvoke.Imwrite("_dat/deb/candi.png", deb1);