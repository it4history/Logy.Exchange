using System;
using System.Drawing;

namespace Logy.Maps.Geometry.Isolines
{
    /// <summary>
    /// http://opita.net/node/755
    /// </summary>
    public class Grehem
    {
        Point p0 = new Point();
        int sizeP;

        public Point[] CalcGrehem(Point[] _A)
        {
            return convex_build(_A);
        }

        /// <summary>
        /// построение саммой оболочки(удаление) "лишних" вершин
        /// </summary>
        Point[] convex_build(Point[] Q)
        {
            //p0 - точка с минимальной координатой у или самая левая из таких точек при наличии совпадений

            var size = Q.Length;

            p0 = Q[0];
            int ind = 0;
            for (int i = 1; i < size; i++)
                if (Q[i].Y > p0.Y) { p0 = Q[i]; ind = i; }
                else
                if (Q[i].Y == p0.Y && Q[i].X < p0.X) { p0 = Q[i]; ind = i; }

            //P остальные точки (все Q кроме р0) 
            sizeP = size - 1;
            Point[] P = new Point[sizeP];
            int j = 0;
            for (int i = 0; i < size; i++)
                if (i != ind)
                { P[j] = Q[i]; j++; }

            P = sort(P);  //сортируем Р в порядке возрастания полярного угла,измеряемого 
            //против часовой стрелки относительно р0

            Point[] S = new Point[size]; //массив,который будет содержать вершины оболочки против часовой стрелки 
            S[0] = p0; S[1] = P[0]; S[2] = P[1];
            int last = 2;
            for (int i = 2; i < sizeP; i++)
            {
                while (last > 0 && angle(S[last - 1], S[last], P[i]) >= 0) last--;
                last++;
                S[last] = P[i];
            }
            return S;
        }

        // сортирует все точки множества в порядке возрастания полярного угла по отнощению к р0
        public Point[] sort(Point[] P)
        {
            bool t = true;
            while (t)
            {
                t = false;
                for (int j = 0; j < sizeP - 1; j++)
                    if (alpha(P[j]) > alpha(P[j + 1]))
                    {
                        Point tmp = new Point();
                        tmp = P[j];
                        P[j] = P[j + 1];
                        P[j + 1] = tmp;
                        t = true;

                    }
                    else
                    if (alpha(P[j]) == alpha(P[j + 1]))
                    {
                        if (P[j].X > P[j + 1].X)
                        {
                            for (int k = j + 2; k < sizeP; k++)
                                P[k - 1] = P[k];
                            sizeP--;
                            t = true;
                        }
                        else
                        if (P[j + 1].X > P[j].X)
                        {
                            for (int k = j + 1; k < sizeP; k++)
                                P[k - 1] = P[k];
                            sizeP--;
                            t = true;
                        }


                    }
            }
            return P;
        }

        /// <summary>
        /// через векторное произведение определяет поворот
        /// (если величина отрицательная - поворот против часовой стрелки, и наоборот)
        /// </summary>
        double angle(Point t0, Point t1, Point t2)
        {
            return (t1.X - t0.X) * (t2.Y - t0.Y) - (t2.X - t0.X) * (t1.Y - t0.Y);
        }

        /// <summary>
        /// считает полярный угол данной точки по отнощению к р0
        /// </summary>
        double alpha(Point t)
        {
            t.X -= p0.X;
            t.Y = p0.Y - t.Y;
            double alph;
            if (t.X == 0) alph = Math.PI / 2;
            else
            {
                if (t.Y == 0) alph = 0;
                else alph = Math.Atan(Convert.ToDouble(t.Y) / Convert.ToDouble(t.X));
                if (t.X < 0) alph += Math.PI;
            }
            return alph;
        }

    }
}