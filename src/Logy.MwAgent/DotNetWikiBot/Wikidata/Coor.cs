using System;
using System.Runtime.Serialization;
using AppConfiguration;
using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    /// <summary>
    /// point on sphere
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
            Globe = original.Globe;
        }

        public Coor(string original) : base(original)
        {
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
        /// from 2PI to 0
        /// 
        /// spherical
        /// </summary>
        [DataMember]
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
        /// http://hist.tk/hw/Reduced_co-latitude
        /// </summary>
        [DataMember]
        public double? Beta
        {
            get { return (90 - Y) / 180 * Math.PI; }
            set { Y = 90 - (((value ?? 0) * 180) / Math.PI); }
        }

        public double? Precision { get; set; }

        public string Globe { get; set; }

        #region metrics
        public double Sum
        {
            get { return X + Y; }
        }
        public double Sqrt
        {
            get { return Math.Sqrt(Sum); }
        }

        public static Coor operator +(Coor a, Coor b)
        {
            return (Coor) new Coor
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
            }.Normalize();
        }

        public static Coor operator -(Coor a, Coor b)
        {
            return (Coor)new Coor
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
            }.Normalize();
        }

        public static Coor operator *(Coor a, Coor b)
        {
            return new Coor
            {
                X = a.X * b.X, // may be overflow
                Y = a.Y * b.Y, // may be overflow
            };
        }

        /*public static double operator /(Coor a, Coor b)
        {
            return (b.X == 0 ? 0 : a.X / b.X)
                   + (b.Y == 0 ? 0 : a.Y / b.Y);
        }*/
        #endregion

    }
}