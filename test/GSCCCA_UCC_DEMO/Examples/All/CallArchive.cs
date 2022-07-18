using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.All
{
    public class CallArchive
    {
        public static async Task<bool> ServiceAsync(string user, string password, int id, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                return await service.CallArchiveFilingAsync(user, password, id);
            }
        }
    }
}