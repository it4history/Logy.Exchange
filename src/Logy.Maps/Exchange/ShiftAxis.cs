using System;
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
            Func<int, int> slowFrames = null,
            Action<int> onFrame = null,
            Func<int, int> timeStepByFrame = null)
        {
            var poleShiftsCount = 10;
            var poleShift = Slow ? Poles.Count : poleShiftsCount;

            Data.DoFrames(
                delegate(int frame)
                {
                    if (frame == 0
                        || (Slow && frame % slowFrames(frame) == 0 && poleShift <= poleShiftsCount))
                    {
                        var newPole = new Datum
                        {
                            X = DesiredDatum.X, /// * slowFrame / slowFramesCount
                            Y = 90 - ((90 - DesiredDatum.Y) * poleShift / poleShiftsCount)
                        };
                        newPole.Gravity = new Gravity { X = newPole.X, Y = newPole.Y };
                        SetDatum(newPole, frame + 1); /// will be applied on next DoFrame()
                        poleShift++;
                    }
                    onFrame?.Invoke(frame);
                    return timeStepByFrame?.Invoke(frame) ?? 15; // 15 for k4, 80 for k5 of Meridian
                },
                framesCount);
        }
    }
}