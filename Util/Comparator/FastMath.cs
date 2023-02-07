namespace FingerprintRecognitionV2.Util.Comparator
{
    static public class FastMath
    {
        static public readonly int
            MaxDistance = 1 << 10,
            MaxAngle = 256;

        static private readonly int[]
            _Sqrt = new int[1<<20],         // Sqrt[i] = Round( Sqrt(i) )
            _Sin = new int[MaxAngle<<1|1],  // Sin[a + MaxAngle] = Round( Sin(a * 2PI / 256) )
            _Cos = new int[MaxAngle<<1|1],  // Cos[a + MaxAngle] = Round( Cos(a * 2PI / 256) )
            _Atan2 = new int[1<<22];        // Atan2[((y + MaxDistance)<<11) | (x + MaxDistance)] = Round( Atan2(y, x) )

        static public int Sqrt(int i) => _Sqrt[i];
        static public int Sin(int a) => _Sin[a + MaxAngle];
        static public int Cos(int a) => _Cos[a + MaxAngle];
        static public int Atan2(int y, int x) => _Atan2[(y + MaxDistance) << 11 | (x + MaxDistance)];

        static public int Cast(double x) => Convert.ToInt32(Math.Round(x));

        static FastMath()
        {
            for (int i = 0; i < (1<<20); i++)
                _Sqrt[i] = Cast(Math.Sqrt(i));

            for (int i = -MaxAngle; i <= MaxAngle; i++)
            {
                _Sin[i + MaxAngle] = Cast(Math.Sin(Math.PI * i / 128));
                _Cos[i + MaxAngle] = Cast(Math.Cos(Math.PI * i / 128));
            }

            for (int y = -MaxDistance; y < MaxDistance; y++)
                for (int x = -MaxDistance; x < MaxDistance; x++)
                    _Atan2[(y + MaxDistance) << 11 | (x + MaxDistance)] = Cast(Math.Atan2(y, x));
        }
    }
}
