using GSCCCA.eFile.Integration.AuthorityWallet;
using GSCCCA.eFile.Integration.Xml;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration
{
    /// <summary>
    /// Service class that makes calls to the Clerks Authority UCC and Account APIs
    /// For use by filers who are submitting new UCCs
    /// </summary>
    /// <remarks>
    /// This should be called in a using statement in order to dispose of the connection when completed.
    /// <code>
    ///         using (var service = new FilerService(loader))
    ///         {
    ///             return await service.CallCheckCredentialsAsync(user, password);
    ///         }
    /// </code>
    /// </remarks>
    public class FilerService : IDisposable
    {
        /// <summary>
        /// When used, will use the default IEndpointLoader to get all endpoint values.
        /// </summary>
        public FilerService() : this("") { }

        /// <summary>
        /// When used, will use the default IEndpointLoader to get the ASMX pages, but the base endpoint is set by the <paramref name="endpointBase"/> parameter.
        /// </summary>
        /// <param name="endpointBase">Defines the address Uri without the ASMX page</param>
        public FilerService(string endpointBase) : this(endpointBase, new ConfigFileEndpointLoader()) { }

        /// <summary>
        /// When used, will use the custom given IEndpoingLoader to get all endpoint values.
        /// </summary>
        /// <param name="loader">Defines how endpoints will be set</param>
        public FilerService(IEndpointLoader loader) : this(string.Empty, loader) { }

        /// <summary>
        /// When used, will load the endpoint addresses using the loader, but it will set the endpoint base given
        /// </summary>
        /// <param name="endpointBase">Defines the address Uri without the ASMX page</param>
        /// <param name="loader">Defines how endpoints will be set</param>
        public FilerService(string endpointBase, IEndpointLoader loader)
        {
            if (string.IsNullOrEmpty(endpointBase))
                endpointBase = loader.LoadEndpointBase();

            Uri endpointUri = new Uri(endpointBase);
            EndpointAddress actEndpoint = new EndpointAddress(endpointBase + loader.LoadAccountEndpoint());
            EndpointAddress uccEndpoint = new EndpointAddress(endpointBase + loader.LoadUCCEndpoint());
            BasicHttpBinding binding = loader.LoadBinding();

            if (endpointUri.Scheme.ToLower() == "https")
                binding.Security.Mode = BasicHttpSecurityMode.Transport;

            UCCService = new org.gsccca.efile.uccservice.UCCServiceSoapClient(binding, uccEndpoint);
            AccountService = new org.gsccca.efile.accountservice.AccountServiceSoapClient(binding, actEndpoint);
        }        

        /// <summary>
        /// Used to check user credentials to ensure the input/loaded values are correct. 
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <returns>true if credentials are valid</returns>
        public async Task<bool> CallCheckCredentialsAsync(string user, string password)
        {
            org.gsccca.efile.accountservice.CheckCredentialsResponse response = await AccountService.CheckCredentialsAsync(user, password);
            return response.Body.CheckCredentialsResult;
        }

        /// <summary>
        /// Gets the GSCCCA eFile Wallet items that can be used as payment on https://efile.gsccca.org
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <returns>Object containing all credit card, ACH, escrow information for efile.gsccca.org</returns>
        public async Task<Wallet> CallQueryFundsAsync(string user, string password)
        {
            org.gsccca.efile.accountservice.QueryFundsResponse response = await AccountService.QueryFundsAsync(user, password);
            return XmlFormatter<Wallet>.Deserialize(response.Body.QueryFundsResult);
        }

        /// <summary>
        /// Deposits funds into an escrow account that can be used as a payment method on https://efile.gsccca.org
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="token">value telling efile.gsccca.org how to pay - found by calling CallQueryFundsAsync</param>
        /// <param name="amount">amount to pay into escrow account</param>
        /// <returns>true if payment is successful and escrow is funded</returns>
        public async Task<bool> CallDepositEscrowAsync(string user, string password, string token, decimal amount)
        {
            org.gsccca.efile.accountservice.DepositDrawdownFundsResponse response = await AccountService.DepositDrawdownFundsAsync(user, password, token, amount);
            return response.Body.DepositDrawdownFundsResult;
        }

        /// <summary>
        /// Determine if a county is participating in UCC.
        /// </summary>
        /// <param name="county">County name, FIPS code, or number</param>
        /// <returns>true indicates you can submit electronic UCCs to the county, false indicates you cannot</returns>
        public async Task<bool> CallIsParticipatingAsync(string county)
        {
            org.gsccca.efile.uccservice.IsParticipatingResponse response = await UCCService.IsParticipatingAsync(county);
            return response.Body.IsParticipatingResult;
        }

        /// <summary>
        /// Cancel a rejected or awaiting payment UCC 
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>aram>
        /// <param name="id">filing identifier of UCC to cancel</param>
        /// <returns>true indicates that the UCC was cancelled</returns>
        /// <remarks>Cancel can only work on UCCs in the Rejected or AwaitingPayment status. Once a UCC
        /// is in Awaiting Clerk Review and visible to the county, it cannot be cancelled or removed from
        /// the county queue. The same applies to Accepted filings.
        /// Cancelling a UCC removes any escrow hold that is on the UCC.
        /// </remarks>
        public async Task<bool> CallCancelAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.CancelResponse response = await UCCService.CancelAsync(user, password, id);
            return response.Body.CancelResult;
        }

        /// <summary>
        /// Submits a new UCC to a county. 
        /// Once submitted, payment must be authorized in order to be visible in the county queue.
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="county">County name, FIPS code, or number</param>
        /// <param name="filingXML">IACA File XML with Georgia modifications</param>
        /// <param name="document">PDF of UCC filing</param>
        /// <returns>Object containing filing data</returns>
        /// <remarks>The PDF document submitted is the official filing, its text superseding the XML.</remarks>
        public async Task<IacaStatus.v3.StatusDocument> CallSubmitAsync(string user, string password, string county, string filingXML, byte[] document)
        {
            org.gsccca.efile.uccservice.SubmitResponse response = await UCCService.SubmitAsync(user, password, county, filingXML, document);
            return XmlFormatter<IacaStatus.v3.StatusDocument>.Deserialize(response.Body.SubmitResult);
        }

        /// <summary>
        /// Resubmits a previously rejected UCC. 
        /// Anything can be changed about  the UCC on resubmittal in order to fix the original filing.
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="county">County name, FIPS code, or number</param>
        /// <param name="filingXML">IACA File XML with Georgia modifications</param>
        /// <param name="document">PDF of UCC filing</param>
        /// <returns>IACA Status and Receipt XML with Georgia modifications</returns>
        public async Task<IacaStatus.v3.StatusDocument> CallResubmitAsync(string user, string password, string county, string filingXML, byte[] document)
        {
            org.gsccca.efile.uccservice.ResubmitResponse response = await UCCService.ResubmitAsync(user, password, county, filingXML, document);
            return XmlFormatter<IacaStatus.v3.StatusDocument>.Deserialize(response.Body.ResubmitResult);
        }

        /// <summary>
        /// Authorizes payment for a submitted UCC that is in the AwaitingPayment status.
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="token">value telling efile.gsccca.org how to pay - found by calling CallQueryFundsAsync</param>
        /// <param name="fee">estimated fee returned when submitted - used to ensure filer knows how much they are paying</param>
        /// <param name="id">filing identifier of UCC to pay</param>
        /// <returns></returns>
        public async Task<bool> CallPayAsync(string user, string password, string token, decimal fee, int id)
        {
            org.gsccca.efile.uccservice.PayResponse response = await UCCService.PayAsync(user, password, token, fee, id);
            return response.Body.PayResult;
        }

        /// <summary>
        /// Gets the current status of a given UCC in the eFile workflow.
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to return</param>
        /// <returns>IACA Reciept and Status XML</returns>
        public async Task<IacaStatus.v3.StatusDocument> CallGetStatusAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.GetStatusResponse response = await UCCService.GetStatusAsync(user, password, id);
            return XmlFormatter<IacaStatus.v3.StatusDocument>.Deserialize(response.Body.GetStatusResult);
        }

        /// <summary>
        /// Archives a UCC on the https://efile.gsccca.org dashboard
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to archive</param>
        /// <returns>true if the filing was archived</returns>
        public async Task<bool> CallArchiveFilingAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.ArchiveUCCResponse response = await UCCService.ArchiveUCCAsync(user, password, id);
            return response.Body.ArchiveUCCResult;
        }

        /// <summary>
        /// Gets the full and most up to date XML data associated with the submitted UCC
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to return</param>
        /// <returns>IACA File XML with Georgia modifications</returns>
        public async Task<IacaFile.v3.FileDocument> CallGetFilingDataAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.GetFilingDataResponse response = await UCCService.GetFilingDataAsync(user, password, id);
            return XmlFormatter<IacaFile.v3.FileDocument>.Deserialize(response.Body.GetFilingDataResult);
        }

        /// <summary>
        /// Gets the PDF document of the given UCC
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to return</param>
        /// <returns>PDF document of filing (stamped if accepted)</returns>
        public async Task<byte[]> CallGetFilingDocumentAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.GetFilingDocumentResponse response = await UCCService.GetFilingDocumentAsync(user, password, id);
            return response.Body.GetFilingDocumentResult;
        }

        private org.gsccca.efile.uccservice.UCCServiceSoapClient UCCService { get; set; }
        private org.gsccca.efile.accountservice.AccountServiceSoapClient AccountService { get; set; }

        #region IDisposable Support
        private bool disposed = false;

        /// <summary>
        /// FilerService should be within a using statement in order to properly dispose of all resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    (UCCService as IDisposable).Dispose();
                    (AccountService as IDisposable).Dispose();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    UCCService.Abort();
                    AccountService.Abort();
                    disposed = true;
                }
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}