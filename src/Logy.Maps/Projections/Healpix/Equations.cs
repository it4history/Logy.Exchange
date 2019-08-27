using System;
using System.Collections.Generic;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Numerics.LinearAlgebra;

namespace Logy.Maps.Projections.Healpix
{
    public class Equations
    {
        /// <summary>
        /// of two types
        /// </summary>
        private readonly int _additionalEquationsCount;

        private readonly WaterMoving<Basin3> _data;

        private readonly List<Equation> _equations = new List<Equation>();

        private List<Node> _nodes;

        public Equations(int lastRing, WaterMoving<Basin3> data)
        {
            _additionalEquationsCount = lastRing - 2;
            _data = data;
        }

        internal Vector<double> Result { get; private set; }

        private bool Readonly => _nodes != null;

        public void Add(Equation equation)
        {
            if (Readonly)
                throw new ApplicationException("readonly");
            _equations.Add(equation);
        }

        public void Solve()
        {
            if (Readonly)
                throw new ApplicationException("readonly");
            _nodes = new List<Node>();
            foreach (var equation in _equations)
            {
                foreach (var node in equation.Nodes)
                {
                    if (!_nodes.Contains(node))
                        _nodes.Add(node);
                }
            }
            _nodes.Sort();

            if (_equations.Count > 0)
            {
                var rowsCount = _equations.Count + (2 * _additionalEquationsCount) - 1;
                var a = Matrix<double>.Build.Dense(
                    rowsCount,
                    _nodes.Count,
                    (row, col) =>
                    {
                        /// koefs 20/2 = 30/2 = 1
                        var additional2dirEquationNum = row - _equations.Count;
                        /// koefs 20/3 = 30/3
                        var additional3dirEquationNum = additional2dirEquationNum - _additionalEquationsCount;

                        var node = _nodes[col];
                        if (additional2dirEquationNum < 0)
                        {
                            var equation = _equations[row];
                            if (equation.Nodes.Contains(node))
                            {
                                return node.Direction.HasValue
                                    ? _data.GetBasinHeight(node.Basin, node.Direction.Value) * node.Koef
                                    : 1;
                            }
                        }
                        else
                        {
                            if (node.Basin.Type.HasValue)
                            {
                                // additional2dirEquationNum >= 0
                                if (additional3dirEquationNum < 0)
                                {
                                    if (additional2dirEquationNum + 2 == node.Basin.Ring && node.Direction == 2)
                                        return 1;
                                }
                                else
                                {
                                    if (node.Direction == 3)
                                    {
                                        if (additional3dirEquationNum + 2 == node.Basin.Ring)
                                            return 1;
                                        if (additional3dirEquationNum + 3 == node.Basin.Ring)
                                            return -1;
                                    }
                                }
                            }
                        }
                        return 0;
                    });
                Result = a.Solve(Vector<double>.Build.Dense(
                    rowsCount,
                    row =>
                    {
                        /// koefs 20/2 = 30/2 = 1
                        var additional2dirEquationNum = row - _equations.Count;
                        /// koefs 20/3 = 30/3
                        var additional3dirEquationNum = additional2dirEquationNum - _additionalEquationsCount;

                        if (additional2dirEquationNum >= 0 && additional3dirEquationNum < 0)
                            return 1;
                        return 0;
                    }));
            }
        }

        /// <summary>converts nodeInRing position to HealpixManager standard</summary>
        /// <param name="nodeInRing">from 0</param>
        public Basin3 GetBasin(int ring, int nodeInRing)
        {
            return _data.PixMan.Pixels[_data.HealpixManager.GetP(
                ring,
                ring - nodeInRing)];
        }

        /// <param name="nodeInRing">from 0</param>
        public double GetResult(int ring, int nodeInRing, int direction)
        {
            if (!Readonly)
                throw new ApplicationException("!readonly");
            return Result[_nodes.IndexOf(
                new Node(GetBasin(ring, nodeInRing)) { Direction = direction })];
        }
    }
}