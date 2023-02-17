
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

        // input constraints
        static public readonly int 
            MinMinutiae = 4,
            MaxMinutiae = 250;
    }
}
