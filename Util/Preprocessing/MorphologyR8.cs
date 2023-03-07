namespace FingerprintRecognitionV2.Util.Preprocessing
{
    public class MorphologyR8 : MorphologyR4
    {
        static public readonly int[] R8Y = { -1, -1, -1, 0, 1, 1, 1, 0 };
        static public readonly int[] R8X = { -1, 0, 1, 1, 1, 0, -1, -1 };

        protected override int GetRY(int t) => R8Y[t];
        protected override int GetRX(int t) => R8X[t];
        protected override int GetRC() => 8;
    }
}
