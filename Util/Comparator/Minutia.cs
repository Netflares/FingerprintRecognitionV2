﻿
namespace FingerprintRecognitionV2.Util.Comparator
{
    public class Minutia
    {
        static private readonly double PI2Byte = 256 / (Math.PI * 2);

        /** 
         * `y` and `x` location are mapped to range [0: 256)
         * `Angle` is mapped from [0: 2PI) to [0: 256)
         * */
        public int Y;
        public int X;
        public int Angle;

        public Minutia() { }

        public Minutia(int y, int x, int angle)
        {
            Y = y;
            X = x;
            Angle = angle;
        }

        public Minutia(int y, int x, double angle)
        {
            Y = y;
            X = x;
            Angle = Convert.ToInt32(angle * PI2Byte);
        }
    }
}
