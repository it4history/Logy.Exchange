using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using AppConfiguration;
using Newtonsoft.Json;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class Coor
    {
        public Coor()
        {
        }

        public Coor(Coor original)
        {
            X = original.X;
            Y = original.Y;
            Precision = original.Precision;
            Globe = original.Globe;
        }

        public Coor(string original)
        {
            var match = Regex.Match(original, @"(-?[\d\.]+):(-?[\d\.]+)");
            if (match.Success)
            {
                X = double.Parse(match.Groups[1].Value);
                Y = double.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// from -180 to 180
        /// </summary>
        [JsonProperty("Longitude")]
        public double X { get; set; }

        /// <summary>
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [JsonProperty("Latitude")]
        public double Y { get; set; }

        /// <summary>
        /// from 0 to 2PI
        /// </summary>
        [DataMember]
        public double? Phi
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
                    throw new ApplicationException("Phi must be from 0 to 2PI");
                }
                X = 180 - (((value ?? 0) * 180) / Math.PI);
            }
        }

        /// <summary>
        /// from 0 to PI, 0 corresponds to North pole
        /// </summary>
        [DataMember]
        public double? Theta
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
            return new Coor
            {
                X = a.X + b.X, 
                Y = a.Y + b.Y, 
            }.Normalize();
        }

        public static Coor operator -(Coor a, Coor b)
        {
            return new Coor
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

        public Coor Normalize()
        {
            while (X < -180) X += 360;
            while (X > 180) X -= 360;
            while (Y < -90) Y += 180;
            while (Y > 90) Y -= 180;
            return this;
        }
    }
}