using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.Maps.Coloring;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;

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
            SetDatum(Poles.Values.Last());

            base.OnDeserialize();
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
                Poles.Add(frame.Value, datum);
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
                        else if (datum.EllipsoidChanged)
                        {
                            var varphi = Ellipsoid.VarphiPaleo(basin, datum.Gravity.Axis);
                            basin.InitROfEllipse(DataAbstract.HealpixManager, Ellipsoid.Radius(varphi));
                            // todo rotate GHpure
                            basin.CalcGpure(varphi, datum);
                        }
                        basin.RecalculateDelta_g(datum);
                    }
                if (frame.HasValue)
                {
                    Poles.Add(frame.Value, datum);
                }
            }
        }
    }
}