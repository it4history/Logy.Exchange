using System;
using System.Linq;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Exchange
{
    public class ShiftAxis : ShiftAxisGeneric<Basin3> 
    {
        public ShiftAxis()
        {
        }

        public ShiftAxis(BasinData dataInited) : base(dataInited)
        {
        }

        [IgnoreDataMember]
        public PoleNorth DesiredPoleNorth { get; set; } = new PoleNorth { X = -40, Y = 73 };

        public bool Slow { get; set; }

        public BasinData Data
        {
            get { return (BasinData)DataAbstract; }
            set { DataAbstract = value; }
        }

        public void Shift(
            int framesCount, 
            Func<int> slowFrames = null,
            Action<int> onFrame = null,
            Func<int, int> timeStepByFrame = null)
        {
            /* SetPole() calculates Delta_g_meridian and Delta_g_traverse 
               on moment Data.Frame but json might be serialized in other moment
               with other Hoq, Radius and therefore other Delta_g, Q3, S_q
            // var pole = Poles.Values.Last();
            // SetPole(pole); */

            var poleShiftsCount = 10;
            var poleShift = Slow
                ? Poles.Count(pole => pole.Key < Data.Frame || Data.Frame == -1)
                : poleShiftsCount;

            Data.DoFrames(
                delegate(int frame)
                {
                    if (frame == 0
                        || (Slow && frame % slowFrames() == 0 && poleShift++ < poleShiftsCount))
                    {
                        var newPole = new PoleNorth
                        {
                            X = DesiredPoleNorth.X, /// * slowFrame / slowFramesCount
                            Y = 90 - ((90 - DesiredPoleNorth.Y) * poleShift / poleShiftsCount)
                        };
                        SetPole(newPole, frame + 1); /// will be applied on next DoFrame()
                    }
                    onFrame?.Invoke(frame);
                    return timeStepByFrame?.Invoke(frame) ?? 15; // 15 for k4, 80 for k5 of Meridian
                },
                framesCount);
        }
    }
}