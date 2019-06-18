using System;
using System.Threading;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.Wos
{
    public class WosManager : IDisposable
    {
        private static Thread _initThread;

        public static void DoWork()
        {
            var man = new HealpixManager(2);
            var world = new World(man);
            using (var reliefSur = new Earth2014Manager(ReliefType.Sur))
            {
                using (var reliefBed = new Earth2014Manager(ReliefType.Bed))
                {
                    using (var reliefIce = new Earth2014Manager(ReliefType.Ice))
                    {
                        for (var p = 0; p < man.Npix; p++)
                        {
                            var coor = man.GetCenter(p);
                            var surface = reliefSur.GetAltitude(coor);
                            var bed = reliefBed.GetAltitude(coor);
                            var relativeHeight = bed - surface;
                            if (relativeHeight != 0)
                            {
                                // water or ice
                                var ice = reliefIce.GetAltitude(coor);
                                if (ice == 0)
                                {
                                    // lakes in ice are ignored
                                    world.AddWater(coor, bed, surface);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Start()
        {
            _initThread = new Thread(DoWork);
            _initThread.Start();
        }

        public void Dispose()
        {
            if (_initThread != null)
                _initThread.Abort();
        }

        public class World
        {
            private readonly HealpixManager _man;
            private readonly Atom[] _atoms;

            public World(HealpixManager man)
            {
                _man = man;
                _atoms = new Atom[man.Npix];
            }

            public void AddWater(HealCoor coor, int bed, int surface)
            {
                _atoms[coor.P] = new Atom();
            }
        }

        internal class Atom
        {
        }
    }
}