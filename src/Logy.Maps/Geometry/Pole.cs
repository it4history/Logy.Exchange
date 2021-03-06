﻿using System.Runtime.Serialization;
using Logy.Maps.Exchange;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Logy.Maps.Geometry
{
    public class Pole : Coor
    {
        public Pole()
        {
            Calc();
        }

        public UnitVector3D Axis { get; private set; }

        public Matrix<double> Matrix { get; private set; }

        #region North pole
        /// <summary>
        /// from -180 to 180, 180 corresponds to East on the right
        /// </summary>
        [DataMember]
        [JsonProperty("X")]
        public override double X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
                Calc();
            }
        }

        /// <summary>
        /// from -90 to 90, 90 corresponds to North pole
        /// </summary>
        [DataMember]
        [JsonProperty("Y")]
        public override double Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
                Calc();
            }
        }
        #endregion

        public HealCoor PoleBasin { get; set; }

        public Bundle<Basin3> LoadCorrection(int k)
        {
            var correctionMap = new OceanMapGravityAxisChange(k);
            var format = $"{correctionMap.Dir}{correctionMap.SubdirByDatum(this)}";
            return Bundle<Basin3>.DeserializeFile(RotationStopMap<Basin3>.FindJson(format), true);
        }

        private void Calc()
        {
            Axis = Utils3D.Cartesian(this);
            Matrix = Matrix3D.RotationTo(BasinAbstract.Oz, Axis);
        }
    }
}