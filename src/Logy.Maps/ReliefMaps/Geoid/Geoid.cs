using System;
using System.Collections.Generic;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid
    {
        /// <summary>
        /// for stabilized accelerations new geoid may be obtained
        /// by modelling its surface with BasinOfGeoid.GeoidRadius
        /// calculated from BasinAbstract.Radius etc on some moment of time
        /// finally ideal is to get mathematical formula for new geoid
        /// </summary>
        public static void Obtain(WaterMoving<BasinOfGeoid> data)
        {
            var polygons = new List<Polygon>();
            foreach (var basin in data.PixMan.Pixels)
            {
                if (basin.P == 0)
                {
                    if (!basin.HasWater())
                        throw new ApplicationException("must have water to know geoid radius");

                    basin.SetGeoid();
                }
                else
                {
                    if (!basin.FillNewGeoid(data.Water))
                        throw new ApplicationException("cannot fill");
                }
            }
        }
    }
}