using System;
using System.Collections.Generic;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Projections.Healpix
{
    /// <summary>
    /// deformation fixer
    /// http://hist.tk/ory/Искажение_начала_перетекания
    /// </summary>
    public class HealpixFormattor
    {
        /// <param name="lastRing">from 2</param>
        /// <returns></returns>
        public Equations Format(int lastRing, WaterMoving<Basin3> data)
        {
            var basins = new Basin3[lastRing - 1, NodesInRing(lastRing)];
            var equations = new Equations(lastRing, data);
            for (var ring = 2; ring <= lastRing; ring++)
            {
                Node firstBasinNode = null;
                for (var nodeInRing = 0; nodeInRing < NodesInRing(ring); nodeInRing++)
                {
                    var basin = equations.GetBasin(ring, nodeInRing);
                    basins[ring - 2, nodeInRing] = basin;

                    if (ring > 2)
                    {
                        if (nodeInRing == 0)
                        {
                            firstBasinNode = new Node(basin);
                            var equation = new Equation
                            {
                                Nodes = new List<Node>
                                {
                                    firstBasinNode,
                                    new Node(basins[ring - 2 - 1, 0], 2) { Koef = -1 }
                                }
                            };
                            if (ring < lastRing)
                                equation.Nodes.AddRange(new[]
                                {
                                    new Node(basin, 2),
                                    new Node(basin, 3)
                                });
                            equations.Add(equation);
                        }
                        else
                        {
                            Equation nextEquationInRing = null;
                            if (nodeInRing < NodesInRing(ring) - 1)
                            {
                            }
                            else
                            {
                                nextEquationInRing = new Equation
                                {
                                    Nodes = new List<Node>
                                    {
                                        new Node(basin),
                                        new Node(basins[ring - 2 - 1, nodeInRing - 1], 3)
                                        {
                                            Koef = -1
                                        }
                                    }
                                };
                                if (NodesInRing(ring - 1) <= nodeInRing)
                                    nextEquationInRing.Nodes.Add(
                                        new Node(basins[ring - 2 - 1, nodeInRing - 1], 3)
                                        {
                                            Koef = -1
                                        });
                                else
                                    nextEquationInRing.Nodes.Add(
                                        new Node(basins[ring - 2 - 1, nodeInRing], 2)
                                        {
                                            Koef = -1
                                        });

                                // water out basin
                                if (ring < lastRing)
                                {
                                    nextEquationInRing.Nodes.Add(
                                        new Node(basin, 2));
                                    nextEquationInRing.Nodes.Add(
                                        new Node(basin, NodesInRing(ring + 1) > nodeInRing + 1 ? 3 : 2));
                                }
                                equations.Add(nextEquationInRing);
                            }

                            // Hoq in the ring are same
                            equations.Add(new Equation
                            {
                                Nodes = new List<Node>
                                {
                                    firstBasinNode,
                                    new Node(nextEquationInRing.Nodes[0])
                                    {
                                        Koef = -1
                                    }
                                }
                            });
                        }
                    }
                }
            }

            equations.Solve();
            return equations;
        }

        private static int NodesInRing(int row)
        {
            return (int)Math.Round(.5 * row);
        }
    }
}