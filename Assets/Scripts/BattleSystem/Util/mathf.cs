namespace BattleSystem
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public struct mathf
    {
        public const float PI = 3.141593f;
        public const float Infinity = float.PositiveInfinity;
        public const float NegativeInfinity = float.NegativeInfinity;
        public const float Deg2Rad = 0.01745329f;
        public const float Rad2Deg = 57.29578f;
        static mathf()
        {
        }

        public static float Sin(float f)
        {
            return (float)Math.Sin((double)f);
        }

        public static float Cos(float f)
        {
            return (float)Math.Cos((double)f);
        }

        public static float Tan(float f)
        {
            return (float)Math.Tan((double)f);
        }

        public static float Asin(float f)
        {
            return (float)Math.Asin((double)f);
        }

        public static float Acos(float f)
        {
            return (float)Math.Acos((double)f);
        }

        public static float Atan(float f)
        {
            return (float)Math.Atan((double)f);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
        }

        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt((double)f);
        }

        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        public static float Min(float a, float b)
        {
            return ((a >= b) ? b : a);
        }

        public static float Min(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
            {
                return 0f;
            }
            float num2 = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static int Min(int a, int b)
        {
            return ((a >= b) ? b : a);
        }

        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }
            int num2 = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static float Max(float a, float b)
        {
            return ((a <= b) ? b : a);
        }

        public static float Max(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
            {
                return 0f;
            }
            float num2 = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static int Max(int a, int b)
        {
            return ((a <= b) ? b : a);
        }

        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }
            int num2 = values[0];
            for (int i = 1; i < length; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }
            return num2;
        }

        public static float Pow(float f, float p)
        {
            return (float)Math.Pow((double)f, (double)p);
        }

        public static float Exp(float power)
        {
            return (float)Math.Exp((double)power);
        }

        public static float Log(float f, float p)
        {
            return (float)Math.Log((double)f, (double)p);
        }

        public static float Log(float f)
        {
            return (float)Math.Log((double)f);
        }

        public static float Log10(float f)
        {
            return (float)Math.Log10((double)f);
        }

        public static float Ceil(float f)
        {
            return (float)Math.Ceiling((double)f);
        }

        public static float Floor(float f)
        {
            return (float)Math.Floor((double)f);
        }

        public static float Round(float f)
        {
            return (float)Math.Round((double)f);
        }

        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling((double)f);
        }

        public static int FloorToInt(float f)
        {
            return (int)Math.Floor((double)f);
        }

        public static int RoundToInt(float f)
        {
            return (int)Math.Round((double)f);
        }

        public static float Sign(float f)
        {
            return ((f < 0f) ? -1f : 1f);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return (a + ((b - a) * Clamp01(t)));
        }

        public static float LerpUnclamped(float a, float b, float t)
        {
            return (a + ((b - a) * t));
        }

        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return (a + (num * Clamp01(t)));
        }

        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Abs((float)(target - current)) <= maxDelta)
            {
                return target;
            }
            return (current + (Sign(target - current) * maxDelta));
        }

        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            target = current + DeltaAngle(current, target);
            return MoveTowards(current, target, maxDelta);
        }

        public static float SmoothStep(float from, float to, float t)
        {
            t = Clamp01(t);
            t = (((-2f * t) * t) * t) + ((3f * t) * t);
            return ((to * t) + (from * (1f - t)));
        }

        public static float Gamma(float value, float absmax, float gamma)
        {
            bool flag = false;
            if (value < 0f)
            {
                flag = true;
            }
            float num = Abs(value);
            if (num > absmax)
            {
                return (!flag ? num : -num);
            }
            float num2 = Pow(num / absmax, gamma) * absmax;
            return (!flag ? num2 : -num2);
        }


        public static float Repeat(float t, float length)
        {
            return (t - (Floor(t / length) * length));
        }

        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return (length - Abs((float)(t - length)));
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }
            return 0f;
        }

        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return num;
        }


        internal static long RandomToLong(System.Random r)
        {
            byte[] buffer = new byte[8];
            r.NextBytes(buffer);
            return (((long)BitConverter.ToUInt64(buffer, 0)) & 0x7fffffffffffffffL);
        }
    }
}

