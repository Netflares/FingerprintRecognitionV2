using Emgu.CV;
using Emgu.CV.Structure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	public class Processor
	{
		/** 
		 * @ public fields
		 * */
		public double[,] NormMat = new double[Param.Height, Param.Width];
		public bool[,] SegmentMsk = new bool[Param.Height, Param.Width];
		public double[,] OrientMat = new double[Param.Height / Param.BlockSize, Param.Width / Param.BlockSize];
		public double[,] OCLMat = new double[Param.Height / Param.BlockSize, Param.Width / Param.BlockSize];
		public bool[,] SkeletonMat = new bool[Param.Height, Param.Width];

		public double WaveLen;

		/** 
		 * @ private fields 
		 * 
		 * allocate memory zone for this class's components
		 * */
		MorphologyR4 morp4 = new();
		MorphologyR8 morp8 = new();
		Orientation orientCalc = new();
		Wavelength waveCalc = new();
		GaborFilter gabor = new();
		MedianFilter medianFilter = new();
		LakeRemover lakeRemover = new();
		ZhangBruteThinning sker;

		/**
		 * @ public methods
		 * */
		public Processor() { sker = new(morp8); }

		// process results will be stored in those public fields
		public void Process(Image<Gray, byte> src, bool smoothMsk = true)
		{
			// @ usage: remove finger pressure differences
			// @ result: avg(NormMat) = 0, std(NormMat) = 1
			Normalization.Normalize(src, NormMat);

			// segmentation
			Segmentation.CreateMask(NormMat, SegmentMsk, Param.BlockSize);

			// orientation
			orientCalc.Create(NormMat, OrientMat, OCLMat);

			// smoothing (must be called before wavelength calculation)
			HandleSmoothing(smoothMsk);

			// wavelength (frequency)
			Normalization.ExcludeBackground(NormMat, SegmentMsk);
			WaveLen = waveCalc.GetMedianWavelength(NormMat, OrientMat, SegmentMsk);

			// gabor filter
			gabor.Apply(NormMat, OrientMat, WaveLen, SegmentMsk, SkeletonMat);
			medianFilter.Exec(SkeletonMat, Param.GaborMedianFilterRadius);
		}

		public void PrepareForExtraction() 
		{
			morp4.Erose(SegmentMsk, Param.BlockSize);
			Segmentation.Padding(SegmentMsk, Param.BlockSize);
			lakeRemover.Exec(SkeletonMat, SegmentMsk, Param.GaborMinimumLakeSize);
			sker.Thinning(SkeletonMat, SegmentMsk);
		}

		private void HandleSmoothing(bool smoothMsk)
		{
			if (smoothMsk) 
			{
				/**
				 * if you want the gabor/skeleton image to look nice for display
				 * you'd want to smooth the mask
				 * */
				morp8.Open(SegmentMsk, Param.BlockSize);
				morp4.Open(SegmentMsk, Param.BlockSize);
				morp4.Close(SegmentMsk, Param.BlockSize);
			}
			else
			{
				/**
				 * otherwise, if you want the mask to contain only useful information
				 * don't smooth it
				 * */
			}
		}
	}
}
