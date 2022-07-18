using GSCCCA.eFile.Integration;
using GSCCCA.eFile.Integration.Xml;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers
{
    public class CallResubmit
    {
        public static async Task<IacaStatus.v3.StatusDocument> ServiceAsync(string user, string password, string county, string filingXml, string documentUri, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                var response = await service.CallResubmitAsync(user: user,
                                                               password: password,
                                                               county: county,
                                                               filingXML: filingXml,
                                                               document: System.IO.File.ReadAllBytes(documentUri));

                return response;
            }
        }
    }
}
