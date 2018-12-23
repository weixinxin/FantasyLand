namespace BattleSystem
{
    using System;
    using System.Reflection;
    public struct vector3
    {
        public const float kEpsilon = 1E-05f;
        public float x;
        public float y;
        public float z;
        public vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        public static vector3 Lerp(vector3 a, vector3 b, float t)
        {
            t =  mathf.Clamp01(t);
            return new  vector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
        }

        public static  vector3 LerpUnclamped( vector3 a,  vector3 b, float t)
        {
            return new  vector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
        }

        public static vector3 Slerp(vector3 from, vector3 to, float t)
        {
            t = mathf.Clamp(t, 0f, 1f);
            var difference = to - from;
            return from + difference * t;
        }


        public static  vector3 MoveTowards( vector3 current,  vector3 target, float maxDistanceDelta)
        {
             vector3 vector = target - current;
            float magnitude = vector.magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0f))
            {
                return (current + (( vector3) ((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }


        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    case 2:
                        return this.z;
                }
                throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    case 2:
                        this.z = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }
        public void Set(float new_x, float new_y, float new_z)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
        }

        public static  vector3 Scale( vector3 a,  vector3 b)
        {
            return new  vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public void Scale( vector3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public static  vector3 Cross( vector3 lhs,  vector3 rhs)
        {
            return new  vector3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
        }

        public override int GetHashCode()
        {
            return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is  vector3))
            {
                return false;
            }
             vector3 vector = ( vector3) other;
            return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
        }

        public static  vector3 Reflect( vector3 inDirection,  vector3 inNormal)
        {
            return ((( vector3) ((-2f * Dot(inNormal, inDirection)) * inNormal)) + inDirection);
        }

        public static  vector3 Normalize( vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return ( vector3) (value / num);
            }
            return zero;
        }

        public void Normalize()
        {
            float num = Magnitude(this);
            if (num > 1E-05f)
            {
                this = ( vector3) (this / num);
            }
            else
            {
                this = zero;
            }
        }

        public  vector3 normalized
        {
            get
            {
                return Normalize(this);
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z };
            return  string.Format("({0:F1}, {1:F1}, {2:F1})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format) };
            return string.Format("({0}, {1}, {2})", args);
        }

        public static float Dot( vector3 lhs,  vector3 rhs)
        {
            return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
        }



        public static float Angle( vector3 from,  vector3 to)
        {
            return (float)( Math.Acos( mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);
        }

        public static float Distance( vector3 a,  vector3 b)
        {
             vector3 vector = new  vector3(a.x - b.x, a.y - b.y, a.z - b.z);
             return (float)Math.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }

        public static  vector3 ClampMagnitude( vector3 vector, float maxLength)
        {
            if (vector.sqrMagnitude > (maxLength * maxLength))
            {
                return ( vector3) (vector.normalized * maxLength);
            }
            return vector;
        }

        public static float Magnitude( vector3 a)
        {
            return (float)Math.Sqrt(((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
            }
        }
        public static float SqrMagnitude( vector3 a)
        {
            return (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z));
        }

        public float sqrMagnitude
        {
            get
            {
                return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
            }
        }
        public static  vector3 Min( vector3 lhs,  vector3 rhs)
        {
            return new  vector3( Math.Min(lhs.x, rhs.x),  Math.Min(lhs.y, rhs.y),  Math.Min(lhs.z, rhs.z));
        }

        public static  vector3 Max( vector3 lhs,  vector3 rhs)
        {
            return new  vector3( Math.Max(lhs.x, rhs.x),  Math.Max(lhs.y, rhs.y),  Math.Max(lhs.z, rhs.z));
        }

        public static  vector3 zero
        {
            get
            {
                return new  vector3(0f, 0f, 0f);
            }
        }
        public static  vector3 one
        {
            get
            {
                return new  vector3(1f, 1f, 1f);
            }
        }
        public static  vector3 forward
        {
            get
            {
                return new  vector3(0f, 0f, 1f);
            }
        }
        public static  vector3 back
        {
            get
            {
                return new  vector3(0f, 0f, -1f);
            }
        }
        public static  vector3 up
        {
            get
            {
                return new  vector3(0f, 1f, 0f);
            }
        }
        public static  vector3 down
        {
            get
            {
                return new  vector3(0f, -1f, 0f);
            }
        }
        public static  vector3 left
        {
            get
            {
                return new  vector3(-1f, 0f, 0f);
            }
        }
        public static  vector3 right
        {
            get
            {
                return new  vector3(1f, 0f, 0f);
            }
        }

        public static  vector3 operator +( vector3 a,  vector3 b)
        {
            return new  vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static  vector3 operator -( vector3 a,  vector3 b)
        {
            return new  vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static  vector3 operator -( vector3 a)
        {
            return new  vector3(-a.x, -a.y, -a.z);
        }

        public static  vector3 operator *( vector3 a, float d)
        {
            return new  vector3(a.x * d, a.y * d, a.z * d);
        }

        public static  vector3 operator *(float d,  vector3 a)
        {
            return new  vector3(a.x * d, a.y * d, a.z * d);
        }

        public static  vector3 operator /( vector3 a, float d)
        {
            return new  vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==( vector3 lhs,  vector3 rhs)
        {
            return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
        }

        public static bool operator !=( vector3 lhs,  vector3 rhs)
        {
            return (SqrMagnitude(lhs - rhs) >= 9.999999E-11f);
        }
    }
}

