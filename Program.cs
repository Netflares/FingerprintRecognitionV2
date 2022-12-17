using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Preprocessing;

ProcImg img = new(
    new Image<Gray, byte>(Constants.DAT_DIR + "i\\0.jpg")
);
