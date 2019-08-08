using System;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid
    {
        /// <summary>
        /// for stabilized accelerations new geoid may be obtained
        /// by modelling its surface with GeoidBasin.RadiusGeoid
        /// calculated from BasinAbstract.Radius etc on some moment of time
        /// finally ideal is to get mathematical formula for new geoid
        /// </summary>
        public static void Obtain<T>(WaterMoving<T> data) where T : Basin3
        {
            foreach (var basin in data.PixMan.Pixels)
            {
                if (!basin.FillNewGeoid(data.Water))
                    throw new ApplicationException("cannot fill");
            }
        }
    }
}