using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.All
{
    public class CallGetFilingDocument
    {
        public static async Task<byte[]> ServiceAsync(string user, string password, int id, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                return await service.CallGetFilingDocumentAsync(user, password, id);
            }
        }
    }
}