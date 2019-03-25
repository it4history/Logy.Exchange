using System.Collections.Generic;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.Wos
{
    public class Water
    {
        /// <summary>
        /// should be sorted manually
        /// 
        /// SortedList performs worse than SortedDictionary during construction 
        /// if the items are not inserted in already-sorted order (https://stackoverflow.com/questions/1376965/when-to-use-a-sortedlisttkey-tvalue-over-a-sorteddictionarytkey-tvalue7)
        /// </summary>
        public SortedList<int, EddyPair> EddiesPair = new SortedList<int, EddyPair>();

        /// <summary>
        /// initial atoms grouping
        /// </summary>
        public void AddAtom(HealCoor coor, int bed, int surface)
        {
            var isAtomAlone = true;

            // look back (right) in the same ring
            if (coor.PixelInRing > 1)
            {
                var otherEddy = GetEddy(coor.P - 1);
                isAtomAlone = false;   
            }

            if (isAtomAlone)
            {
                EddiesPair.Add(coor.P, new EddyPair(coor));
            }
        }

        private EddyPair GetEddy(int p)
        {
            var beforeFirstGreaterOrEqual = EddiesPair.FindFirstIndexGreaterThanOrEqualTo(p) - 1;
            if (beforeFirstGreaterOrEqual >= 0)
            {
                return EddiesPair[beforeFirstGreaterOrEqual];
            }
            return null;
        }
    }

    public class EddyPair
    {
        /// <summary>
        /// kilometers
        /// </summary>
        public int MaxRadius = 50;

        public EddyPair(HealCoor coor)
        {
        }
    }
}