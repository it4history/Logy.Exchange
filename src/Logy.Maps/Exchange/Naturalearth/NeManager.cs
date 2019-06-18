using System.IO;

namespace Logy.Maps.Exchange.Naturalearth
{
    public class NeManager
    {
        public static string Filepath => Path.Combine(
            "Exchange\\Naturalearth", 
            "ne_110m_admin_0_countries.geojson" /*"110m.json"*/);
    }
}