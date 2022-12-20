using System.Runtime.Serialization;

namespace Vorp.Shared.Models
{
    [DataContract]
    public class Position
    {
        [DataMember(Name = "x")]
        public float X { get; set; }

        [DataMember(Name = "y")]
        public float Y { get; set; }

        [DataMember(Name = "z")]
        public float Z { get; set; }

        [DataMember(Name = "heading")]
        public float H { get; set; }

        public Position()
        {
        }

        public Position(float x, float y, float z, float heading)
        {
            X = x;
            Y = y;
            Z = z;
            H = heading;
        }

        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Position Subtract(Position position)
        {
            X = X - position.X;
            Y = Y - position.Y;
            Z = Z - position.Z;
            H = H - position.H;

            return this;
        }

        public Position Add(Position position)
        {
            X = X + position.X;
            Y = Y + position.Y;
            Z = Z + position.Z;
            H = H + position.H;

            return this;
        }

        public Position Clone()
        {
            return new Position(X, Y, Z, H);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class VectorExtensions
    {
        public static Position ToPosition(this Vector3 vector3, float heading = 0f)
        {
            return new Position(vector3.X, vector3.Y, vector3.Z, heading);
        }
    }

    public class RotatablePosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public RotatablePosition(float x, float y, float z, float yaw, float pitch, float roll)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }
    }
}
