using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Meridian.Data
{
    public class StartCheckingGeodesic : MeridianData<MeridianCoor>
    {
        private int _maxH;

        public StartCheckingGeodesic(HealpixManager man) : base(man)
        {
            ColorsMiddle = 0;
        }

        public override double? GetAltitude(MeridianCoor basin)
        {
            //basin.PreInit(HealpixManager);

            double? diff = null;
            var northBasin = GetNorthBasin(basin);
            if (northBasin != null)
            {
                double more, less;
                basin.IntersectGeodesic(northBasin, out more, out less);

                //diapazon 17..22m when k=9 when * r * HealpixManager.ThetaPix 
                var diffAngle = Math.Abs(Math.Abs(basin.Vartheta) - Math.Abs(northBasin.Vartheta));
                //return diffAngle * r * HealpixManager.ThetaPix;
                //r * diffAngle is 8.3..11.1km when k=9, is more accurate on equator
                //return r * diffAngle;

                //diapazon 16.5..22.2m for k=9 when * r * HealpixManager.ThetaPix 
                /*var diffThetaAngle = theta - northBasin.theta;
                diffThetaAngle = diffThetaAngle > Math.PI ? diffThetaAngle - Math.PI : diffThetaAngle;*/
                //return diffThetaAngle * r * HealpixManager.ThetaPix;

                //var Avert = r > northBasin.r ? basin.Am : northBasin.Am;
                //var AvertLess = r > northBasin.r ? northBasin.Am : basin.Am;
                var diffAnglePer2 = diffAngle / 2; // results are very the same if diffThetaAngle
                var MNLess = Triangles.SinusesTheorem(Math.PI / 2 //+ AvertLess
                    , less, diffAnglePer2);
                var MNMore = Triangles.SinusesTheorem(Math.PI / 2 //- Avert
                    , more, diffAnglePer2);

                diff = MNMore - MNLess;

            }
            return diff;
        }

        internal void GetDiff(MeridianCoor basin, MeridianCoor northBasin = null)
        {
            /*some calculations

            // 5.5m for k = 11, 11m for k = 10, 22m for k = 9, 80(44 maxH)m for k = 8
            var hmeters = r * HealpixManager.ThetaPix * hTan;
            if (hmeters > maxH)
            {
                maxH = hmeters;
            }
            */

            /*var more = r > northBasin.r ? r : northBasin.r;
            var less = r > northBasin.r ? northBasin.r : r;
            //return more - less - hmeters;//diapazon 15.1m
            //return Math.Cos(diffAngle) * more - less - hmeters;//diapazon 13.4m
            */

            /*obsolete calculations
            //diapazon of diffMeters = Math.Abs(R_vartheta - northBasin.R_vartheta) 
            //  is 75m for k=8; 37m for k=9
            var r_eDiff = Math.Abs(r - northBasin.r);
            //but diffMeters - r_eDiff
            //  is 2m for k=8; 1m for k=9
            var diffMeters = Math.Abs(R_vartheta - northBasin.R_vartheta);
            //return diffMeters;
            //equiRadius may be calculated by http://hist.tk/hw/Числа_Лава or by http://hist.tk/hw/Коэффициент_упругости
            //var diff = equiRadius - r;
            return (EllipsoidAcceleration.GOnPoles - gPureToCenter - aFromCenter)*r;
            return Ellipsoid.BigRadius * EllipsoidAcceleration.GWithAOnEquator
                   - r * gPureToCenter;
            */
        }

        public override void Log()
        {
            Console.WriteLine(_maxH);
            //diapazon 7m for k=5, 3.5m for k=6, 1.75m for k=7, 0.88m for k=8 and 0.44m when k=9
            var diapazon = 7 * Math.Pow(0.5, HealpixManager.K - 5) * 1.1;
            Assert.LessOrEqual(Colors.Max, diapazon / 2);
            Assert.GreaterOrEqual(Colors.Min, -diapazon / 2);
            base.Log();
        }
    }
}