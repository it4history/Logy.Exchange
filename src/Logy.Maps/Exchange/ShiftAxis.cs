using System;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Exchange
{
    public class ShiftAxis : ShiftAxisGeneric<Basin3> 
    {
        public ShiftAxis()
        {
        }

        public ShiftAxis(OceanData dataInited) : base(dataInited)
        {
        }

        [IgnoreDataMember]
        public Datum DesiredDatum { get; set; } = new Datum { X = -40, Y = 73 };

        public bool Slow { get; set; }

        public bool Geoisostasy { get; set; }

        public OceanData Data
        {
            get { return (OceanData)DataAbstract; }
            set { DataAbstract = value; }
        }

        public void Shift(
            int framesCount, 
            Action<int> onFrame = null,
            Func<int, int> slowFrames = null,
            Func<int, int> timeStepByFrame = null)
        {
            var poleShiftsCount = 10;
            var poleShift = Slow ? Poles.Count : poleShiftsCount;
            if (slowFrames == null)
                slowFrames = (frame) => 2;

            Data.DoFrames(
                delegate(int frame)
                {
                    var slowAnyChange = (frame * (Geoisostasy ? 2 : 1)) % slowFrames(frame) == 0;
                    if (frame == 0
                        || (Slow && poleShift <= poleShiftsCount && slowAnyChange))
                    {
                        var datum = Poles.Last().Value;
                        var newX = DesiredDatum.X; /// * slowFrame / slowFramesCount
                        var newY = 90 - ((90 - DesiredDatum.Y) * poleShift / poleShiftsCount);
                        var slowCentrifugalChange = frame % slowFrames(frame) == 0;
                        if (!Slow || slowCentrifugalChange)
                        {
                            datum = new Datum
                            {
                                X = newX,
                                Y = newY,
                                Gravity = datum.Gravity
                            };
                            if (!Geoisostasy)
                                poleShift++;
                        }
                        if (Geoisostasy && (!Slow || !slowCentrifugalChange))
                        {
                            datum = new Datum
                            {
                                X = datum.X,
                                Y = datum.Y,
                                GravityFirstUse = true,
                                Gravity = new Gravity { X = newX, Y = newY }
                            };
                            poleShift++;
                        }
                        SetDatum(datum, frame + 1); /// will be applied on next DoFrame()
                    }
                    onFrame?.Invoke(frame);
                    return timeStepByFrame?.Invoke(frame) ?? 15; // 15 for k4, 80 for k5 of Meridian
                },
                framesCount);
        }
    }
}