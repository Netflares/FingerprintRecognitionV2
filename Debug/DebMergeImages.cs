
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;

namespace FingerprintRecognitionV2.Debug
{
	static class DebMergeImages
	{
		static public Image<Bgr, byte> DebOCL(string fImg, string fSke, string fOcl)
		{
			Image<Gray, byte> img = new(fImg), ocl = new(fOcl);
			Image<Bgr, byte>  ans = new(fSke);

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
	}
}
