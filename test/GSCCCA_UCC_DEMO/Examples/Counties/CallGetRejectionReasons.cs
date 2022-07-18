using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Counties
{
    public class CallGetRejectionReasons
    {
        public static async Task<string[]> ServiceAsync(string endpoint)
        {
            using (CountyService service = new CountyService(endpoint))
            {
                return await service.CallGetRejectionReasonsAsync();
            }
        }
    }
}
