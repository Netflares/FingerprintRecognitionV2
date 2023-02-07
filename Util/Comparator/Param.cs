
namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class Param
    {
        static public readonly int
            LocalDistanceTolerance = 20,
            GlobalDistanceTolerance = 20,
            AngleTolerance = 128 / 4;   // 45deg

        static public readonly int
            ToleranceProduct = LocalDistanceTolerance * AngleTolerance * AngleTolerance;
    }
}
