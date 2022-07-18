using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Counties
{
    public class CallReject
    {
        public static async Task<bool> ServiceAsync(string user, string password, int filingid, List<string> reasons, string endpoint)
        {
            using (CountyService service = new CountyService(endpoint))
            {
                return await service.CallRejectAsync(user: user,
                                                     password: password,
                                                     id: filingid,
                                                     reasons: reasons.ToArray());
            }
        }
    }
}
