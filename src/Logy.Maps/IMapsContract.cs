using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.Projections.Healpix.Dem;

namespace Logy.Maps
{
    /// <summary>
    /// http://logy.gq/lw/Category:Earth
    /// </summary>
    [ServiceContract]
    public interface IMapsContract
    {
        /// <param name="inside">x-y in degrees</param>
        /// <param name="level">meters</param>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Map{inside}_{level}", ResponseFormat = WebMessageFormat.Json)]
        List<HealCoor> Map(string inside, string level);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Earth{to}_{locale}", ResponseFormat = WebMessageFormat.Json)]
        List<HealCoor> Earth(string to, string locale);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Healpix{k}", ResponseFormat = WebMessageFormat.Json)]
        List<HealCoor> Healpix(string k);

        /// <summary>
        /// returns Dem for datum
        /// </summary>
        /// <param name="datum">name from http://logy.gq/lw/Category:Datums</param>
        /// <param name="k">resolution</param>
        /// <param name="parentK">to filter datum by parent pixels</param>
        /// <param name="parentP">P of parent basin or list of P separated by commas</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Dem_{datum}_{k}_{parentK}_{parentP}", ResponseFormat = WebMessageFormat.Json)]
        HealDem Dem(string datum, string k, string parentK, string parentP);
    }
}