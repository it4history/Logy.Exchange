using System;
using System.Runtime.Serialization;
using AppConfiguration;
using Newtonsoft.Json;

namespace Logy.MwAgent.Sphere
{
    /// <summary>
    /// point on sphere
    /// by default X == -180, Y == 90
    /// </summary>
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class Coor : Point2
    {
        public Coor()
        {
        }

        public Coor(Point2 original) : base(original)
        {
        }

        public Coor(Coor original) : base(original)
        {
            Precision = original.Precision;
        }

        public Coor(string original) : base(original)
        {
        }

        public Coor(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// from -180 to 180, 180 corresponds to East on the right
        /// </summary>
        [JsonProperty("Longitude")]
        public override double X
        {
            get { return base.X - 180; }
            set { base.X = value + 180; }
        }

        /// <summary>
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [JsonProperty("Latitude")]
        public override double Y
        {
            get { return 90 - base.Y; }
            set { base.Y = 90 - value; }
        }

        /// <summary>
        /// from 2PI on West to 0 on East, counterclockwise, λ
        /// 
        /// spherical
        /// http://skills.gq/aw/Axises_orientation#Z_up
        /// Pi      x = r   y = 0
        /// Pi/2    x = 0   y = r
        /// 0       x = -r  y = 0
        /// 1.5Pi   x = 0  y = -r
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public double? Lambda
        {
            get
            {
                return ((180 - X) / 180) * Math.PI;
            }
            set
            {
                if (value.HasValue && value > 2 * Math.PI)
                {
                    // value = -2 * Math.PI;
                    throw new ApplicationException("Lambda must be from 0 to 2PI");
                }
                X = 180 - (((value ?? 0) * 180) / Math.PI);
            }
        }

        /// <summary>
        /// from 0 to PI, 0 corresponds to North pole
        /// 
        /// spherical
        /// http://hist.tk/ory/Reduced_co-latitude
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public double? Beta
        {
            get { return base.Y / 180 * Math.PI; }
            set { Y = 90 - (((value ?? 0) * 180) / Math.PI); }
        }

        /// <summary>
        /// Pi/2 .. -Pi/2; Pi/2 is North pole
        /// </summary>
        public double Phi
        {
            get
            {
                return (Math.PI * .5) - Beta.Value;
            }
        }

        public double? Precision { get; set; }

        #region operators

        public static Coor operator +(Coor a, Coor b)
        {
            return new Coor(a.X + b.X, a.Y + b.Y).Normalize<Coor>();
        }

        public static Coor operator -(Coor a, Coor b)
        {
            return new Coor(a.X - b.X, a.Y - b.Y).Normalize<Coor>();
        }

        public static Coor operator *(Coor a, Coor b)
        {
            // may be overflow
            return new Coor(a.X * b.X, a.Y * b.Y);
        }
        #endregion
    }
}