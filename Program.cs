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

int matches = Matcher.Match(probe, candi);
Console.WriteLine(matches);
