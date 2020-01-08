using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.Maps.Coloring;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Exchange
{
    public class ShiftAxisGeneric<T> : Algorithm<T> where T : BasinAbstract
    {
        private const double Pole2BasinAccuranceDegrees = 1.5;
        public ShiftAxisGeneric()
        {
        }

        public ShiftAxisGeneric(WaterMoving<T> dataInited) : base(dataInited)
        {
        }

        /// <summary>
        /// key - frame when pole used
        /// </summary>
        public Dictionary<int, Datum> Poles { get; set; } = new Dictionary<int, Datum>
        {
            {
                -1, Datum.Normal
            }
        };

        public override void OnDeserialize()
        {
            /* SetDatum() calculates Delta_g_meridian and Delta_g_traverse 
               on moment Data.Frame but 
               for 2) calc method http://hist.tk/ory/Способ_расчета_центробежного_ускорения
                  json might be serialized in other moment
                  with other Hoq, Radius and therefore other Delta_g, Q3, S_q, 
                  so SetDatum() will not be accurate if Delta_g depends on Hoq via a Basin3.Q3 in EllipsoidAcceleration.Centrifugal() */
            SetDatum(Poles.Values.Last()); /// SetGeoisostasyDatum called manually

            base.OnDeserialize();
        }

        /// <summary>
        /// used when json loaded and calc continueted not from 0 frame
        /// </summary>
        public void SetGeoisostasyDatum(ShiftAxisGeneric<T> algo = null, int? correctionK = null)
        {
            if (algo == null)
                algo = this;
            var lastPoleFrame = algo.Poles.Keys.Last();
            var datum = algo.Poles[lastPoleFrame];
            datum.CorrectionBundle = datum.Gravity.LoadCorrection(correctionK ?? algo.DataAbstract.K);
            SetDatum(datum, lastPoleFrame);
        }

        public void SetDatum(Datum datum, int? frame = null)
        {
            if (DataAbstract?.PixMan != null)
            {
                var poleWasSet = datum.PoleBasin != null;
                if (!poleWasSet)
                    foreach (var b in DataAbstract.PixMan.Pixels)
                    {
                        if (Math.Abs(b.X - datum.X) < Pole2BasinAccuranceDegrees &&
                            Math.Abs(b.Y - datum.Y) < Pole2BasinAccuranceDegrees)
                        {
                            datum.PoleBasin = b;
                            break;
                        }
                    }

                DataAbstract.AdditionalDraw += (healCoor, bmp, point, scale) =>
                {
                    if (datum.PoleBasin != null)
                    {
                        double r, width;

                        switch (DataAbstract.K)
                        {
                            default:
                            case 7:
                                r = poleWasSet ? .1 : .01;
                                width = poleWasSet ? .01 : .03;
                                break;
                            case 6:
                                r = poleWasSet ? .08 : .03;
                                width = .02;
                                break;
                            case 5:
                                r = .2;
                                width = .06;
                                break;
                        }

                        var dist = datum.PoleBasin.DistanceTo(healCoor);
                        if (DataAbstract.Colors != null
                            && dist >= r - width && dist <= r + width)
                        {
                            ColorsManager.SetPixelOnBmp(
                                Color.FromArgb(255, 174, 201),
                                bmp,
                                point,
                                scale);
                        }
                    }
                };
            }

            ChangeRotation(datum, null, 0);

            if (frame.HasValue)
            {
                Poles[frame.Value] = datum;
            }
        }

        public void ChangeRotation(int? frame, double koef = double.MaxValue)
        {
            ChangeRotation(new Datum(), frame, koef);
        }

        private void ChangeRotation(Datum datum, int? frame = null, double koef = double.MaxValue)
        {
            if ((koef > 0 && datum.SiderealDayInSeconds < double.MaxValue / 2)
                || -koef < datum.SiderealDayInSeconds)
            {
                datum.SiderealDayInSeconds += koef;
                if (DataAbstract?.PixMan != null)
                    foreach (var basin in DataAbstract.PixMan.Pixels)
                    {
                        if (DataAbstract.SamePolesAndEquatorGravitation)
                            basin.GHpure = 0; /// what about GVpure?
                        else if (!datum.GravityNormal)
                        {
                            var varphi = Ellipsoid.VarphiPaleo(basin, datum.Gravity.Axis);

                            /*
                            var beta = (Math.PI / 2) - varphi;
                            var thetaTan = Ellipsoid.CalcThetaTan(beta);
                            var theta = Math.Atan(thetaTan);
                            varphi = (Math.PI / 2) - theta;*/
                            var theta = (Math.PI / 2) - varphi;

                            var vartheta = Ellipsoid.CalcVarTheta(Math.Tan(theta));
                            var delta_gq = theta - vartheta;
                            var gh = basin.CalcGpureAndInitROfGeoid(
                                DataAbstract.HealpixManager,
                                varphi, 
                                theta, 
                                vartheta, 
                                BasinAbstract.GoodDeflection(vartheta, delta_gq));
                            var correction = datum.CorrectionBundle;
                            if (correction != null)
                            {
                                basin.RadiusOfGeoid += correction.Basins[correction.Algorithm.DataAbstract.K]
                                    [correction.Algorithm.DataAbstract.K == DataAbstract.K ? basin.P : basin.ParentP]
                                    .Hoq;
                            }

                            // GHpure projections
                            /*was
                            var basinSphere = //basin.S_sphere;
                                new Plane(Matrixes.ToCartesian(new Coor(0, 180/Math.PI* varphi)));

                            var oz_sphere = Basin3.Oz.ProjectOn(basinSphere).Direction;
                            var axis_sphere = datum.Gravity.Axis.ProjectOn(basinSphere).Direction;
                            var angle = oz_sphere.SignedAngleTo(axis_sphere, basinSphere.Normal);
                            var gh_sphere = new Vector3D(0, Math.Sign(vartheta) * gh, 0) * Matrix3D.RotationAroundZAxis(angle);
                            basin.GHpure = Math.Sign(basin.Vartheta) * gh_sphere[1];
                            basin.GHpureTraverse = gh_sphere[0];//*/

                                //*
                            var axisPlane = new Plane(BasinAbstract.O3, basin.Qgeiod, datum.Gravity.Axis.ToPoint3D());
                            var axis_sphere = axisPlane.IntersectionWith(basin.S_sphere).Direction;//S_sphere may be sphere without Radius
                            /*var correctionAngle = Math.PI / 2 - axis_sphere.AngleTo(basin.RadiusLine).Radians;
                            var ghCorrected = gh / Math.Cos(correctionAngle);*/
                            if (axis_sphere.DotProduct(datum.Gravity.Axis) < 0)
                                axis_sphere = axis_sphere.Negate();
                            if (vartheta < 0)
                                axis_sphere = axis_sphere.Negate();
                            var gh_sphere = axis_sphere.ScaleBy(gh) * basin.Matrix;
                            basin.GHpure = Math.Sign(basin.Vartheta) * gh_sphere[2];
                            basin.GHpureTraverse = gh_sphere[1]; //*/
                        }
                        basin.RecalculateDelta_g(datum, false);
                    }
                if (frame.HasValue)
                {
                    Poles.Add(frame.Value, datum);
                }
            }
        }
    }
}