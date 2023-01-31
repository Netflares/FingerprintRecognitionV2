using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

Stopwatch timer = new();
timer.Start();

for (int i = 0; i < 500; i++)
{
	Image<Gray, byte> src = new("_dat/set00/" + i + ".bmp");
	ProcImg img = new(src);
	CvInvoke.Imwrite(
		"_dat/visualize-ver5/" + i.ToString("D3") + ".png", 
		ProcImg.Visualize(ProcImg.SkeletonMat, img.Minutiae)
	);
	img.Export("_dat/inp-ver5/" + i.ToString("D3") + ".inp");
}

timer.Stop();
ProcImg.PrintTime(timer, "proc 500 img:");
