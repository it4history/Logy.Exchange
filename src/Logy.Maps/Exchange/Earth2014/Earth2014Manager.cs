using System;
using System.Collections.Generic;
using System.IO;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Exchange.Earth2014
{
    public class Earth2014Manager : IDisposable
    {
        /// <summary>
        /// like Ellipsoid.MeanRadius
        /// </summary>
        public const int Radius2Add = 6371000;

        public const int AltitudeAccuracy = 2; // meters

        private readonly bool _readAllAtStart;

        /// <summary>
        /// used if !_readAllAtStart
        /// </summary>
        private readonly FileStream _stream;

        /// <summary>
        /// used if _readAllAtStart
        /// </summary>
        private readonly byte[] _buf;

        private readonly ReliefType _reliefType;

        /// <summary>
        /// 1 or 5
        /// </summary>
        private readonly int _accuracyMin;

        private readonly bool _shape;

        private readonly Coor _center;

        /* complex for me now
        private readonly int[] _buf;
        private readonly ValueCoor _bufStart;
        private readonly ValueCoor _bufEnd;
        private static void FillBuffer(ValueCoor center, int radiusGrad)
        {
            _bufStart = new ValueCoor
            {
                Longitude = center.Longitude - radiusGrad,
                Latitude = center.Latitude - radiusGrad
            };
            _bufEnd = new ValueCoor
            {
                Longitude = center.Longitude + radiusGrad,
                Latitude = center.Latitude + radiusGrad
            };
            var size = _bufEnd - _bufStart;
            _buf = new int[size];
            using (var stream = new FileStream(Filepath, FileMode.Open))
            {
                var i = radiusGrad * (60 / _accuracyMin);
                for (var row = -i; row < i; row++)
                {
                    for (var column = -i; column < i; column++)
                    {
                        var c = new ValueCoor
                        {
                            Longitude = center.Longitude - radiusGrad,
                            Latitude = center.Latitude - radiusGrad
                        };
                        stream.Position = c.Offset * 2;
                        var a = stream.ReadByte();
                        var b = stream.ReadByte();
                        var int16 = (UInt16) (a << 8) + b;
                        if (a >= 0x80)
                            _buf[i] = -((int16 ^ 0xffff) + 1);
                        else
                            _buf[i] = int16;
                    }
                }
            }
        }*/

        public Earth2014Manager(ReliefType type, int accuracyMin = 5, bool shape = false, bool readAllAtStart = false)
        {
            if (type == ReliefType.Mask && accuracyMin != 1)
                throw new ApplicationException("ReliefType.Mask in 1 accuracy only");
            _readAllAtStart = readAllAtStart;
            _reliefType = type;
            _accuracyMin = accuracyMin;
            _shape = shape;
            if (_readAllAtStart)
            {
                using (var stream = new FileStream(Filepath, FileMode.Open))
                {
                    _buf = new byte[stream.Length];
                    stream.Read(_buf, 0, (int)stream.Length);
                }
            }
            else
            {
                _stream = new FileStream(Filepath, FileMode.Open, FileAccess.Read);
            }
        }

        public Earth2014Manager(Coor center, int accuracyMin = 1, int radiusGrad = 10)
            : this(ReliefType.Bed, accuracyMin)
        {
            _center = center;
            /// FillBuffer(center, radiusGrad);
        }

        private string Filepath
        {
            /// get { return Path.Combine("earth2014", "Earth2014Shape_minus_6371000m.BED2014.5min.geod.bin"); }
            get
            {
                var fileName = string.Format(
                    "Earth2014{2}.{1}2014.{0}min.geod.bin",
                    _accuracyMin,
                    _reliefType.ToString().ToUpper(),
                    _shape ? "Shape_minus_6371000m" : string.Empty);
                return Path.Combine($"Exchange{Path.DirectorySeparatorChar}Earth2014", fileName);
            }
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }
        }

        public List<HealCoor> GetLevelDots(int level, double angleFromCenter = 5)
        {
            var l = new List<HealCoor>();
            for (double latitude = _center.Y - angleFromCenter;
                latitude < _center.Y + angleFromCenter;
                latitude += _accuracyMin / 60.0)
            {
                for (double longitude = _center.X - angleFromCenter;
                    longitude < _center.X + angleFromCenter;
                    longitude += _accuracyMin / 60.0)
                {
                    var coor = new HealCoor { X = longitude, Y = latitude };
                    var diff = GetAltitude(coor) - level;
                    if (Math.Abs(diff) <= AltitudeAccuracy)
                    {
                        l.Add(coor);
                    }
                }
            }

            return l;
        }

        public List<object> GetPerimeter(int level)
        {
            // look for west and then tracking path north, east, south
            var westStart = new HealCoor(_center);
            int diff;
            do
            {
                diff = GetAltitude(westStart) - level;
                westStart.X -= _accuracyMin / 60.0;
            } while (Math.Abs(diff) > AltitudeAccuracy);

            var l = new List<object>();
            return l;
        }

        public int GetAltitude(HealCoor place)
        {
            var position = place.Offset(_accuracyMin) * 2;
            int a, b;
            if (_readAllAtStart)
            {
                a = _buf[position];
                b = _buf[position + 1];
            }
            else
            {
                _stream.Position = position;
                a = _stream.ReadByte();
                b = _stream.ReadByte();
            }
            var int16 = (ushort)(a << 8) + b;
            if (a >= 0x80)
                return -((int16 ^ 0xffff) + 1);
            return int16;
        }
    }
}