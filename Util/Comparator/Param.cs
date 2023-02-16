
namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class Param
    {
        static public readonly double
            LocalDistanceTolerance = 20,
            GlobalDistanceTolerance = 20,
            AngleTolerance = Math.PI / 4;   // 45deg

        static public readonly double
            ToleranceProduct = LocalDistanceTolerance * AngleTolerance * AngleTolerance;
    }
}
