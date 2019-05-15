﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Exchange
{
    public class ShiftAxis : Algorythm<Basin3>
    {
        public ShiftAxis(BasinData data) : base(data)
        {
        }

        [IgnoreDataMember]
        public PoleNorth DesiredPoleNorth { get; set; } = new PoleNorth { X = -40, Y = 73 };

        public bool Slow { get; set; }

        /// <summary>
        /// key - frame
        /// </summary>
        public Dictionary<int, PoleNorth> Poles { get; set; } = new Dictionary<int, PoleNorth>
        {
            {
                -1, new PoleNorth { X = 0, Y = 90 }
            }
        };
    }
}