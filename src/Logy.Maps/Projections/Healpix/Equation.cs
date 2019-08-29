using System.Collections.Generic;
using System.Text;

namespace Logy.Maps.Projections.Healpix
{
    public class Equation
    {
        public List<Node> Nodes { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var node in Nodes)
            {
                sb.Append(node.Koef == 1 ? "+" : (node.Koef == -1 ? "-" : node.Koef.ToString()));
                sb.Append(node);
            }
            return sb.ToString();
        }
    }
}