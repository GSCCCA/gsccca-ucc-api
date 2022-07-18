using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Counties
{
    public class CallGetQueue
    {
        public static async Task<string[]> ServiceAsync(string user, string password, string endpoint)
        {
            using (CountyService service = new CountyService(endpoint))
            {
                return await service.CallGetQueueAsync(user: user, password: password);
            }
        }
    }
}
