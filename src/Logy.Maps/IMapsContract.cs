using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Logy.Maps.Projections.Healpix;

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
    }
}