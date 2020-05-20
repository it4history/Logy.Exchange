using System;
using System.Drawing;

namespace Logy.Maps.Coloring
{
    public class ColorWheel
    {
        public const double ModuleAccuracy = .005;

        /// <param name="hue">from 0 to 255, 0 - red north, 63 - green east</param>
        /// <param name="satupation">from 0 to 255, 0 is grey</param>
        /// <param name="v">maximum brightness, 255 is most</param>
        public static Color HSVtoRGB(double hue, double satupation = 0, int v = 255)
        {
            if (v == 0)
                return Color.Black;
            if (satupation == 0)
                return Color.FromArgb(v, v, v);

            hue += 256 / 4;
            hue %= 256;

            var maxV = v;
            var chroma = (satupation / 255d) * maxV;
            var minV = maxV - chroma;
            double tempH;
            double r, g, b;
            if (hue >= 170)
            {
                tempH = (hue - 170) / 43;
                g = 0;
                if (tempH < 1)
                {
                    b = maxV;
                    r = maxV * tempH;
                }
                else
                {
                    r = maxV;
                    b = maxV * (2 - tempH);
                }
            }
            else if (hue >= 85)
            {
                tempH = (hue - 85) / 43;
                r = 0;
                if (tempH < 1)
                {
                    g = maxV;
                    b = maxV * tempH;
                }
                else
                {
                    b = maxV;
                    g = maxV * (2 - tempH);
                }
            }
            else
            {
                tempH = hue / 43;
                b = 0;
                if (tempH < 1)
                {
                    r = maxV;
                    g = maxV * tempH;
                }
                else
                {
                    g = maxV;
                    r = maxV * (2 - tempH);
                }
            }
            r = ((r / maxV) * (maxV - minV)) + minV;
            g = ((g / maxV) * (maxV - minV)) + minV;
            b = ((b / maxV) * (maxV - minV)) + minV;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        public static void Draw(Bitmap bmp, int width, PointF leftTop = new PointF())
        {
            var maxBright = width / 2d;
            for (var angle = .0; angle < Math.PI * 2; angle += .01)
            for (int bright = width / 30; bright < maxBright; bright++)
            {
                var x = (width / 2) - (Math.Cos(angle) * bright);
                var y = (width / 2) - (Math.Sin(angle) * bright);
                bmp.SetPixel(
                    (int)(x + leftTop.X),
                    (int)(y + leftTop.Y),
                    HSVtoRGB(255 * angle / (Math.PI * 2), 255 * (bright / maxBright)));
            }
        }

        /// <summary>
        /// angle is million part of value, coded from 0 to 360
        /// </summary>
        /// <returns>0..255</returns>
        public static double GetAngle(double value)
        {
            var module = (int)(value * 1000);
            return ((value - (module / 1000d)) / 360) * 255 * 1000000;
        }

        /// <summary>
        /// angle coding 
        /// 0 - north
        /// Pi/2 - east
        /// </summary>
        /// <param name="meridian">positive for North, in radians</param>
        /// <param name="traverse">positive for East, in radians</param>
        /// <returns>module and angle in gedrees * 0.000001</returns>
        public static double SetAngle(double meridian, double traverse, double module = 0)
        {
            double angleRad;
            if (meridian == 0)
                angleRad = traverse > 0 ? Math.PI / 2 : Math.PI * 1.5;
            else
            {
                var s = Math.Atan(traverse / meridian);
                angleRad = meridian > 0 ? s : Math.PI + s;
                if (angleRad < 0)
                    angleRad += Math.PI * 2;
            }
            var moduleBig = (int)(module * 1000);
            return (moduleBig / 1000d) + ((angleRad / Math.PI) * 180 * 0.000001);
        }
    }
}
