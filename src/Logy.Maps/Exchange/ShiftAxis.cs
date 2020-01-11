using System;
using System.Runtime.Serialization;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Exchange
{
    public class ShiftAxis : ShiftAxisGeneric<Basin3> 
    {
        public ShiftAxis()
        {
        }

        public ShiftAxis(BasinDataAbstract<Basin3> dataInited) : base(dataInited)
        {
        }

        [IgnoreDataMember]
        public Datum DesiredDatum { get; set; } = Datum.Greenland17;

        public bool Slow { get; set; }

        public BasinDataAbstract<Basin3> /*was OceanData*/ Data
        {
            get { return (BasinDataAbstract<Basin3>)DataAbstract; }
            set { DataAbstract = value; }
        }

        /// <summary>
        /// call SetDatum before running, it is not called in OnDeserialize
        /// </summary>
        public bool Geoisostasy { get; set; }

        /* commented because of ReliefAxis17Geoisostasy.AxisChange smoothing
        public override void OnDeserialize()
        {
            if (Geoisostasy)
            {
                var datum = Poles.Values.Last();
                datum.CorrectionBundle = datum.LoadCorrection(Data.K);
            }
            base.OnDeserialize();
        }*/

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
                        var datum = FromLastPole;
                        var newX = DesiredDatum.X; /// * slowFrame / slowFramesCount
                        var newY = 90 - ((90 - DesiredDatum.Y) * poleShift / poleShiftsCount);
                        var slowCentrifugalChange = frame % slowFrames(frame) == 0;
                        if (!Slow || slowCentrifugalChange)
                        {
                            datum.X = newX;
                            datum.Y = newY;
                            if (!Geoisostasy)
                                poleShift++;
                        }
                        if (Geoisostasy && (!Slow || !slowCentrifugalChange))
                        {
                            datum.X = newX;
                            datum.Y = newY;
                            datum.GravityFirstUse = true;
                            datum.Gravity = new Gravity { X = newX, Y = newY };
                            poleShift++;

                            datum.CorrectionBundle = datum.Gravity.LoadCorrection(Data.K);
                        }
                        SetDatum(datum, frame + 1); /// will influence water on next DoFrame()
                    }
                    onFrame?.Invoke(frame);
                    return timeStepByFrame?.Invoke(frame) ?? 15; // 15 for k4, 80 for k5 of Meridian
                },
                framesCount);
        }
    }
}