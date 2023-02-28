using Emgu.CV;
using Emgu.CV.Structure;

namespace FingerprintRecognitionV2.Util.Preprocessing
{
	public class Processor
	{
		/** 
		 * @ public fields
		 * 
		 * are used for storing result images
		 * */
		public double[,] NormMat = new double[Param.Height, Param.Width];
		public bool[,] SegmentMsk = new bool[Param.Height, Param.Width];
		public double[,] OrientMat = new double[Param.Height / Param.BlockSize, Param.Width / Param.BlockSize];
		public bool[,] SkeletonMat = new bool[Param.Height, Param.Width];

		public double WaveLen;

		/** 
		 * @ private fields 
		 * 
		 * allocate memory zone for this class's components
		 * */
		MorphologyR4 morp4 = new();
		Orientation orientCalc = new();
		Wavelength waveCalc = new();
		GaborFilter gabor = new();

		/**
		 * @ public methods
		 * */
		public Processor()
		{

		}

		// process results will be stored in those public fields
		public void Process(Image<Gray, byte> src)
		{
			// @ usage: remove finger pressure differences
			// @ result: avg(NormMat) = 0, std(NormMat) = 1
			Normalization.Normalize(src, NormMat);

			// segmentation
			Segmentation.CreateMask(NormMat, SegmentMsk, Param.BlockSize);
			Segmentation.SmoothMask(SegmentMsk, Param.BlockSize, morp4);

			// orientation
			orientCalc.Create(NormMat, OrientMat);

			// wavelength (frequency)
			Normalization.ExcludeBackground(NormMat, SegmentMsk);
			WaveLen = waveCalc.GetMedianWavelength(NormMat, OrientMat, SegmentMsk);

			// gabor filter
            gabor.Apply(NormMat, OrientMat, WaveLen, SegmentMsk, SkeletonMat);
		}

		public void PrepareForExtraction() 
		{
			ZhangBruteThinning.Thinning(SkeletonMat);
            morp4.Erose(SegmentMsk, Param.BlockSize);
            Segmentation.Padding(SegmentMsk, Param.BlockSize);
		}
	}
}
