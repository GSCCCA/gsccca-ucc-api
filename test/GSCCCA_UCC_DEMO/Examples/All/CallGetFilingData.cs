using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.All
{
    public class CallGetFilingData
    {
        public static async Task<IacaFile.v3.FileDocument> ServiceAsync(string user, string password, int id, string endpoint)
        {
            using (var service = new FilerService(endpoint))
            {
                return await service.CallGetFilingDataAsync(user, password, id);
            }
        }
    }
}