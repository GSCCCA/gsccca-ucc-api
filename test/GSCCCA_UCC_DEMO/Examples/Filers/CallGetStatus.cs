using GSCCCA.eFile.Integration.Xml;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers
{
    public class CallGetStatus
    {
        public static async Task<IacaStatus.v3.StatusDocument> ServiceAsync(string user, string password, int id, string endpoint)
        {
            using (var service = new FilerService(endpoint))
            {
                return await service.CallGetStatusAsync(user, password, id);
            }
        }
    }
}
