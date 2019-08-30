using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Numerics.LinearAlgebra;

namespace Logy.Maps.Projections.Healpix
{
    /// <summary>
    /// koefs for http://hist.tk/ory/Геометрическое_искажение
    /// </summary>
    public class HealpixFormattor<T> where T : Basin3
    {
        private readonly int _lastRing;

        private readonly BasinDataAbstract<T> _data;

        private readonly List<Equation> _equations = new List<Equation>();


        public HealpixFormattor(BasinDataAbstract<T> data, int? lastRingSet = null)
        {
            _lastRing = lastRingSet ?? data.HealpixManager.Nside;
            _data = data;
        }

        public List<Node> Nodes { get; private set; }

        internal Vector<double> Result { get; private set; }

        private bool Readonly => Nodes != null;

        public HealpixFormattor<T> Format()
        {
            var basins = new Basin3[_lastRing - 1, NodesInRing(_lastRing)];
            for (var ring = 2; ring <= _lastRing; ring++)
            {
                Node firstBasinNode = null;
                for (var nodeInRing = 0; nodeInRing < NodesInRing(ring); nodeInRing++)
                {
                    var basin = GetBasin(ring, nodeInRing);
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
                            if (ring < _lastRing)
                                equation.Nodes.AddRange(new[]
                                {
                                    new Node(basin, 2),
                                    new Node(basin, 3)
                                });
                            Add(equation);
                        }
                        else
                        {
                            var nextEquationInRing = new Equation
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
                            if (SymmetricVert(ring, nodeInRing))
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
                            if (ring < _lastRing)
                            {
                                nextEquationInRing.Nodes.Add(
                                    new Node(basin, 2));
                                nextEquationInRing.Nodes.Add(
                                    new Node(basin, SymmetricVert(ring, nodeInRing) ? 2 : 3));
                            }
                            Add(nextEquationInRing);

                            // Hoq in the ring are same
                            Add(new Equation
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

            Solve();
            return this;
        }

        public double GetResult(T basin, int direction)
        {
            var fromPole = _data.HealpixManager.RingFromPole(basin);
            if (basin.NorthCap == null
                || fromPole == 1)
                return 1;

            if (basin.NorthCap == false)
            {
                basin = _data.PixMan.Pixels[_data.HealpixManager.GetP(
                    fromPole,
                    basin.PixelInRing)];
                direction = (int)NeighborManager.GetOppositeVer((Direction)direction);
            }

            if (direction < 2)
            {
                var neighbor = (T)basin.Neighbors[direction];
                direction = basin.Opposites[direction];
                if (direction < 2 || neighbor.Ring == 1)
                {
                    // water moving between extreme basins
                    return 1;
                }
                basin = neighbor;
            }

            var ring = basin.Ring;
            if (ring == _data.HealpixManager.Nside)
            {
                // ring near equator directed to equator
                return 1;
            }

            var nodeInRing = (basin.PixelInRing - 1) % ring;
            nodeInRing = ring - nodeInRing - 1; // PixelInRing order correction
            if (nodeInRing >= NodesInRing(ring))
            {
                nodeInRing = ring - nodeInRing - 1;
                direction = (int)NeighborManager.GetOppositeHor((Direction)direction);
            }
            if (SymmetricVert(ring, nodeInRing) && direction == 3)
            {
                direction = 2;
            }
            return GetResult(ring, nodeInRing, direction);
        }

        /// <summary>
        /// righter than symmetric calculated by (nodeInRing >= NodesInRing(ring))
        /// </summary>
        private bool SymmetricVert(int ring, int nodeInRing)
        {
            return ring <= nodeInRing * 2 + 1;
        }

        /// <summary>only for north half of 1/4 part</summary>>
        /// <param name="ring">from 1 to _lastRing</param>
        /// <param name="nodeInRing">from 0 to NodesInRing(ring)</param>
        internal double GetResult(int ring, int nodeInRing, int direction)
        {
            if (!Readonly)
                throw new ApplicationException("!readonly");
            var basin = GetBasin(ring, nodeInRing);
            return Result[Nodes.IndexOf(
                new Node(basin) { Direction = direction })];
        }

        /// <summary>converts nodeInRing position to HealpixManager standard</summary>
        /// <param name="ring">from 1 to _lastRing</param>
        /// <param name="nodeInRing">from 0 to NodesInRing(ring)</param>
        protected Basin3 GetBasin(int ring, int nodeInRing)
        {
            return _data.PixMan.Pixels[_data.HealpixManager.GetP(
                ring,
                ring - nodeInRing)];
        }

        protected void Add(Equation equation)
        {
            if (Readonly)
                throw new ApplicationException("readonly");
            _equations.Add(equation);
        }

        private static int NodesInRing(int row)
        {
            return (int)Math.Round(.5 * row, MidpointRounding.AwayFromZero);
        }

        private void Solve()
        {
            Nodes = new List<Node>();
            foreach (var equation in _equations)
            {
                foreach (var node in equation.Nodes)
                {
                    if (!Nodes.Contains(node))
                        Nodes.Add(node);
                }
            }
            Nodes.Sort();

            var dic = new Dictionary<EquationType, int>();
            for (int row = 0; row < Nodes.Count; row++)
            {
                int equationNum;
                dic[GetEquationType(row, out equationNum)] = equationNum + 1;
            }
            var rowsCount = (from max in dic.Values select max).Sum();

            if (rowsCount > 0)
            {
                var a = Matrix<double>.Build.Dense(
                    rowsCount,
                    Nodes.Count,
                    (row, col) =>
                    {
                        var node = Nodes[col];
                        var ring = node.Basin.Ring;
                        int equationNum;
                        switch (GetEquationType(row, out equationNum))
                        {
                            case EquationType.Normal:
                                var equation = _equations[row];
                                var sum = 0d;
                                foreach (var equationNode in equation.Nodes)
                                {
                                    if (equationNode.Equals(node))
                                    {
                                        sum += node.Direction.HasValue
                                            ? _data.GetBasinHeight(node.Basin, node.Direction.Value) * equationNode.Koef
                                            : equationNode.Koef;
                                    }
                                }
                                return sum;
                            case EquationType.Koefs20_2:
                                if (node.NodeInRing == 0 && node.Direction == 2
                                    && equationNum + 2 == ring)
                                    return 1;
                                break;
                            case EquationType.Koefs20_3:
                                if (node.NodeInRing == 0 && node.Direction == 3)
                                {
                                    if (equationNum + 2 == ring)
                                        return 1;
                                    if (equationNum + 3 == ring)
                                        return -1;
                                }
                                break;
                            case EquationType.Koefs41_3:
                                if (node.NodeInRing == 1 && node.Direction == 3)
                                {
                                    if (equationNum + 4 == ring)
                                        /// if 1 then 41_2,50_2,51_2=1.1 61_3=1.2 eddy 62-73-83-72-62 
                                        /// if 1.5 then 40_2 = 1.2 eddy less
                                        /// if 2 then 40_2 = 1.3
                                        return 1.5; 
                                    if (equationNum + 5 == ring)
                                        return -1;
                                }
                                break;
                            case EquationType.KoefsLast1_2:
                                if (node.NodeInRing == 1 && node.Direction == 2)
                                {
                                    if (equationNum + _lastRing - 1 == ring)
                                        return 1;
                                }
                                break;
                            case EquationType.KoefsLast0_3:
                                if (node.NodeInRing == 0 && node.Direction == 3)
                                {
                                    if (equationNum + _lastRing - 1 == ring)
                                        return 1;
                                }
                                break;
                            case EquationType.Koefs52_2:
                                if (node.Direction == 2 //&& node.Direction == 2 + (ring-1) % 2
                                )
                                {
                                    if (node.NodeInRing == 2 && equationNum + 5 == ring)
                                        return 1;
                                    if (node.NodeInRing == 3 && equationNum + 7 == ring)
                                        return 1;
                                }
                                break;
                        }
                        return 0;
                    });

                Result = a.Solve(Vector<double>.Build.Dense(
                    rowsCount,
                    row =>
                    {
                        int equationNum;
                        switch (GetEquationType(row, out equationNum))
                        {
                            case EquationType.Koefs20_2:
                                return 1;
                            case EquationType.KoefsLast1_2:
                                return .6;
                            case EquationType.KoefsLast0_3:
                                return .4;
                            case EquationType.Koefs52_2:
                                return .3; //if 1 then 51_3 low // equationNum / 3d + .3;
                            default:
                                return 0;
                        }
                    }));
            }
        }

        private EquationType GetEquationType(int row, out int equationNum)
        {
            var newNum = row - _equations.Count;
            if (newNum >= 0)
            {
                equationNum = newNum;
                newNum -= _lastRing - 2;
                if (newNum < 0)
                    return EquationType.Koefs20_2;

                equationNum = newNum;
                newNum -= _lastRing - 4; // -3 if KoefsLast1_2 is 0;
                if (newNum < 0)
                    return EquationType.Koefs20_3;

                equationNum = newNum;
                newNum -= 1; 
                if (newNum < 0)
                    return EquationType.KoefsLast1_2;

                equationNum = newNum;
                newNum -= 0;
                if (newNum < 0)
                    return EquationType.KoefsLast0_3;

                equationNum = newNum; 
                return EquationType.Koefs41_3; // works on lastring 8 and Data(3) with eddy 62-72-83-73-62

                return EquationType.Koefs52_2; // if KoefsLast0_3 is 0   1 for 6rings, 2 - 7, 4 - 8
            }
            equationNum = row;
            return EquationType.Normal;
        }
    }
}