
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Debug
{
	static class DebMergeImages
	{
		static public Image<Bgr, byte> DebOCL(string fImg, string fSke, string fOcl, bool[,]? msk = null, bool bin = false)
		{
			Image<Gray, byte> img = new(fImg), ocl = new(fOcl);
			Image<Bgr, byte>  ans = new(fSke);

			if (bin) Binarize(ocl);
			if (msk != null) Iterator2D.Forward(msk, (y, x) => ocl[y, x] = new Gray(
					(Convert.ToInt32(!msk[y, x]) * 255) | Convert.ToInt32(ocl[y, x].Intensity)
			));

			Iterator2D.Forward(img, (y, x) => 
			{
				Bgr px = ans[y, x];
				double alter = img[y, x].Intensity;
				if (px.Red + px.Green + px.Blue == 0) {
					ans[y, x] = new Bgr(
						0.6*alter + 0.4*ocl[y, x].Intensity, 
						0.6*alter + 0.4*ocl[y, x].Intensity, 
						0.6*alter
					);
				}
			});
			return ans;
		}

		static private void Binarize(Image<Gray, byte> ocl)
		{
			double thresh = FingerprintRecognitionV2.Util.Preprocessing.Param.OCLThreshold * 255;
			Iterator2D.Forward(ocl, (y, x) => 
			{
				double lvl = ocl[y, x].Intensity <= thresh ? 0 : 255;
				ocl[y, x] = new Gray(lvl);
			});
		}
	}


	/*
	paste this to Program::Debug()
	
	static void Debug() {
		string dImg = "_dat/set/", dSke = "_dat/ske/", dOcl = "_dat/ocl/", dAns = "_dat/ocl05-msk-visual/";

		string[] imgs = Directory.GetFiles(dImg);
		Directory.CreateDirectory(dAns);

		// allocate threads
		Threading<Processor>(imgs.Length, 5, (proc, i) =>
		{
			proc.Process(new Image<Gray, byte>(imgs[i]), false);

			string fname = Path.GetFileName(imgs[i]);
			string ansPath = Path.Join(dAns, fname);
			
			Image<Bgr, byte> ans = FingerprintRecognitionV2.Debug.DebMergeImages.DebOCL(
				dImg + fname, dSke + fname, dOcl + fname, proc.SegmentMsk, true
			);

			CvInvoke.Imwrite(ansPath, ans);
		});
	}
	*/
}
