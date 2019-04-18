using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Coloring
{
    public class ColorsManager
    {
        private readonly SortedList<int, Color3> Haxby = new SortedList<int, Color3>
        {
            { 0, new Color3(37, 57, 175) },
            { 10, new Color3(40, 127, 251) },
            { 20, new Color3(50, 190, 255) },
            { 30, new Color3(106, 235, 255) },
            { 40, new Color3(138, 236, 174) },
            { 50, new Color3(205, 255, 162) },
            { 60, new Color3(240, 236, 121) },
            { 70, new Color3(255, 189, 87) },
            { 80, new Color3(255, 161, 68) },
            { 90, new Color3(255, 186, 133) },
            { 100, new Color3(255, 255, 255) }
        };

        internal static Color3 High = new Color3(200, 200, 200);

        public static readonly SortedList<int, Color3> Elevation = new SortedList<int, Color3>
        {
            { 0, new Color3(0, 191, 191) },
            { 20, new Color3(0, 255, 0) },
            { 40, new Color3(255, 255, 0) },
            { 60, new Color3(255, 127, 0) },
            { 80, new Color3(191, 127, 63) },
            { 100, High }
        };

        internal static Color3 Red = new Color3(Color.Red);

        public static readonly SortedList<int, Color3> Gyr1 = new SortedList<int, Color3>
        {
            { 0, new Color3(Color.Green) },
            { 10, new Color3(Color.Yellow) },
            { 50, new Color3(Color.SandyBrown) },
            { 100, Red },
        };

        internal static Color3 Blue = new Color3(0, 240, 255);
        internal static Color3 DarkBlue = new Color3(0, 50, 170);
        internal static Color3 WaterBorder = new Color3(Color.Aquamarine);

        public static readonly SortedList<int, Color3> Water = new SortedList<int, Color3>
        {
            { 0, Blue },
            { 100, DarkBlue },
        };

        public double Max { get; set; }
        public double Min { get; set; }

        private readonly double? _middleSet;

        public double Middle
        {
            get { return _middleSet ?? (Max + Min) / 2; }
        }

        public double NormalKoef
        {
            get { return 255 / (Max - Min); }
        }

        public bool IsGrey { get; private set; }
        private SortedList<int, Color3> _above = Gyr1;
        private SortedList<int, Color3> _under = Water;
        public Color DefaultColor = Color.BurlyWood;

        public ColorsManager(double min, double max, double? middle = null, bool isGrey = false)
        {
            Min = min;
            Max = max;
            _middleSet = middle;
            IsGrey = isGrey;
        }

        public void SetScales(SortedList<int, Color3> above, SortedList<int, Color3> under)
        {
            _above = above;
            _under = under;
        }

        /// <param name="nearestIndex">may be in next ring and may be == maxIndex</param>
        /// <returns>if false then index X of funcX is between nearestIndex-1 and nearestIndex</returns>
        public static bool FindNearest(double funcX, Func<int, double> func, bool asc, double precision, int maxIndex,
            ref int nearestIndex)
        {
            // if (nearestIndex == 0 && func(nearestIndex) < funcX)
            while (nearestIndex < maxIndex
                   && (asc && func(nearestIndex) < funcX || !asc && func(nearestIndex) > funcX))
            {
                nearestIndex++;
            }

            if (nearestIndex < maxIndex && Math.Abs(funcX - func(nearestIndex)) < precision)
            {
                return true;
            }

            return false;
        }

        /// <param name="deltas">should be >= 0</param>
        public static double LinearApproximation(KeyValuePair<int, double>[] deltas, Func<int, double> func)
        {
            if (deltas.Length == 1)
                return func(deltas[0].Key);
            var sum = (from d in deltas select d.Value).Sum(); //deltas[0].Value + deltas[1].Value;
            return (from d in deltas
                    select func(d.Key) * (1 - d.Value / sum))
                .Sum(); // for 3 and more points incorrect ->   / deltas.Count; 
        }

        public Color3 Get(double value)
        {
            if (_under == null)
                return FindNearestColor((value - Min) / (Max - Min), _above ?? Haxby);
            if (value <= Middle && Min != Middle)
            {
                return FindNearestColor((value - Middle) / (Min - Middle), _under);
            }

            return Max - Middle == 0
                ? new Color3(Color.Black)
                : FindNearestColor((value - Middle) / (Max - Middle), _above);
        }

        private Color3 FindNearestColor(double iPercent, SortedList<int, Color3> colors)
        {
            iPercent *= 100;
            if (iPercent > 100)
                iPercent = 100;
            if (iPercent < 0)
                iPercent = 0;
            int i1 = 0;
            if (FindNearest(iPercent, i => colors.Keys[i], true, .01, colors.Count, ref i1))
            {
                return colors.Values[i1];
            }

            // LinearApproximation var deltas = new Dictionary<int, double>();
            var delta1 = iPercent - colors.Keys[i1 - 1];
            var delta2 = colors.Keys[i1] - iPercent;
            var sum = delta1 + delta2;
            return colors.Values[i1 - 1] + delta1 / sum * (colors.Values[i1] - colors.Values[i1 - 1]);
        }

        public void SetPixelOnBmp(double height, Bitmap bmp, Point2 point, int scale)
        {
            SetPixelOnBmp(height, bmp, (int)point.X, (int)point.Y, scale);
        }

        public void SetPixelOnBmp(double? height, Bitmap bmp, int x, int y, int scale)
        {
            /*var normal = new Vector3(0, 0, 1);
            if (deltas.Length == 2)
            {
                var a = pixMan.GetVector(deltas[0]);
                var b = pixMan.GetVector(deltas[1]);
                var c = pixMan.GetVector(coor, height);
                normal = Vector3.Normalize(Vector3.Cross(a - b, b - c));
            }
            //normal = (255 / (float)2) * (normal + new Vector3(1, 1, 1));*/

            var color = DefaultColor;
            if (IsGrey)
            {
                if (height.HasValue)
                {
                    var grey = (int)((height.Value - Min) * NormalKoef);
                    color = Color.FromArgb(grey, grey, grey);
                }
            }
            else
            {
                if (height.HasValue) color = (Color)Get(height.Value);
            }

            if (scale == 1)
                bmp.SetPixel(x, y, color);
            else
            {
                var scaleX = scale;
                var scaleY = scale;
                var g = Graphics.FromImage(bmp);
                var r = new RectangleF(x * scaleX, y * scaleY, scaleX, scaleY);
                g.FillRectangle(new SolidBrush(color), r);
                g.Flush();
            }
        }
    }
}