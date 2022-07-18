using GSCCCA.eFile.Integration;
using GSCCCA.eFile.Integration.AuthorityWallet;
using GSCCCA.eFile.Integration.UCC.Demo.Examples.All;
using GSCCCA.eFile.Integration.UCC.Demo.Examples.Counties;
using GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers;
using GSCCCA.eFile.Integration.Xml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace GSCCCA_UCC_DEMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtURL.Text = ConfigurationManager.AppSettings["ENDPOINT"];
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            string user = txtUser.Text;
            string pass = txtPassword.Text;
            string endpoint = txtURL.Text;

            try
            {
                msg = await CallCheckCredentials.ServiceAsync(user, pass, endpoint);
                MessageBox.Show(msg, "Connection");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        #region Filer Workflow Tabs Funds Examples
        private async void BtnQueryFunds_Click(object sender, EventArgs e)
        {
            Wallet userwallet = null;
            string user = txtUser.Text;
            string pass = txtPassword.Text;
            string endpoint = txtURL.Text;

            try
            {
                userwallet = await CallQueryFunds.ServiceAsync(user, pass, endpoint);

                txtCCType.Text = userwallet.CreditCards.FirstOrDefault()?.Type;
                txtCCNumber.Text = userwallet.CreditCards.FirstOrDefault()?.Number;
                txtCCExpiration.Text = userwallet.CreditCards.FirstOrDefault()?.Expiration;
                txtCCToken.Text = userwallet.CreditCards.FirstOrDefault()?.Token;

                txtBAName.Text = userwallet.BankAccounts.FirstOrDefault()?.AccountName;
                txtBAAccount.Text = userwallet.BankAccounts.FirstOrDefault()?.AccountNumber;
                txtBARouting.Text = userwallet.BankAccounts.FirstOrDefault()?.RoutingNumber;
                txtBAToken.Text = userwallet.BankAccounts.FirstOrDefault()?.Token;

                txtEscrowAvailable.Text = userwallet.Escrow.AvailableBalance;
                txtEscrowPayments.Text = userwallet.Escrow.PendingPayments;
                txtEscrowDeposits.Text = userwallet.Escrow.PendingDeposits;
                txtEscrowPosted.Text = userwallet.Escrow.PostedBalance;
                txtEscrowToken.Text = userwallet.Escrow.Token;

                MessageBox.Show("QueryFunds() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void btnDepositFundsEscrow_Click(object sender, EventArgs e)
        {
            Wallet userwallet = null;
            bool success = false;
            string user = txtUser.Text;
            string pass = txtPassword.Text;
            string endpoint = txtURL.Text;
            string token = txtFundingToken.Text;
            decimal.TryParse(txtFundingAmount.Text, out decimal amount);

            try
            {
                success = await CallDepositDrawdownFunds.ServiceAsync(user, pass, token, amount, endpoint);
                if (success)
                {
                    userwallet = await CallQueryFunds.ServiceAsync(user, pass, endpoint);

                    txtCCType.Text = userwallet.CreditCards.FirstOrDefault()?.Type;
                    txtCCNumber.Text = userwallet.CreditCards.FirstOrDefault()?.Number;
                    txtCCExpiration.Text = userwallet.CreditCards.FirstOrDefault()?.Expiration;
                    txtCCToken.Text = userwallet.CreditCards.FirstOrDefault()?.Token;

                    txtBAName.Text = userwallet.BankAccounts.FirstOrDefault()?.AccountName;
                    txtBAAccount.Text = userwallet.BankAccounts.FirstOrDefault()?.AccountNumber;
                    txtBARouting.Text = userwallet.BankAccounts.FirstOrDefault()?.RoutingNumber;
                    txtBAToken.Text = userwallet.BankAccounts.FirstOrDefault()?.Token;

                    txtEscrowAvailable.Text = userwallet.Escrow.AvailableBalance;
                    txtEscrowPayments.Text = userwallet.Escrow.PendingPayments;
                    txtEscrowDeposits.Text = userwallet.Escrow.PendingDeposits;
                    txtEscrowPosted.Text = userwallet.Escrow.PostedBalance;
                    txtEscrowToken.Text = userwallet.Escrow.Token;
                }

                MessageBox.Show("DepositDrawdownFunds() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }
        #endregion

        #region Filer Workflow Tabs Submit Examples
        private void BtnLoadSubmitFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogUCCForm = new OpenFileDialog();

            if (openFileDialogUCCForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtUCCFormToSubmit.Text = openFileDialogUCCForm.FileName;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
        private void BtnLoadSubmitXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogXml = new OpenFileDialog();

            if (openFileDialogXml.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader reader = new StreamReader(openFileDialogXml.FileName);
                    txtXmlToSubmit.Text = (reader.ReadToEnd());
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
        private void BtnSaveSubmitXml_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialogXml = new SaveFileDialog();

            if (openFileDialogXml.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(openFileDialogXml.FileName, string.Join("\n", txtXmlToSubmit.Lines));
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                GSCCCA.eFile.Integration.IacaStatus.v3.StatusDocument iacaStatus = await CallSubmit.ServiceAsync(txtUser.Text,
                                                               txtPassword.Text,
                                                               txtCountyToSubmit.Text,
                                                               txtXmlToSubmit.Text,
                                                               txtUCCFormToSubmit.Text,
                                                               txtURL.Text);


                txtSubmitResponseCallDate.Text = iacaStatus.Header.Date.Text.First();
                txtSubmitResponseFilingID.Text = iacaStatus.Record.First().DocumentReceiptID.Text.First();
                txtSubmitResponseFee.Text = iacaStatus.Record.First().EstimatedFee.Text.First();
                txtSubmitResponseStausDate.Text = iacaStatus.Record.First().StatusDate.Text.First();
                txtSubmitResponseStatus.Text = iacaStatus.Record.First().Status.value.ToString();


                MessageBox.Show("Submit() called");

            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnResubmit_Click(object sender, EventArgs e)
        {
            try
            {
                GSCCCA.eFile.Integration.IacaStatus.v3.StatusDocument iacaStatus = await CallResubmit.ServiceAsync(txtUser.Text,
                                                               txtPassword.Text,
                                                               txtCountyToSubmit.Text,
                                                               txtXmlToSubmit.Text,
                                                               txtUCCFormToSubmit.Text,
                                                               txtURL.Text);


                txtSubmitResponseCallDate.Text = iacaStatus.Header.Date.Text.First();
                txtSubmitResponseFilingID.Text = iacaStatus.Record.First().DocumentReceiptID.Text.First();
                txtSubmitResponseFee.Text = iacaStatus.Record.First().EstimatedFee.Text.First();
                txtSubmitResponseStausDate.Text = iacaStatus.Record.First().StatusDate.Text.First();
                txtSubmitResponseStatus.Text = iacaStatus.Record.First().Status.value.ToString();


                MessageBox.Show("Resubmit() called");

            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnPayUCC_Click(object sender, EventArgs e)
        {
            try
            {
                bool response = await CallPay.ServiceAsync(txtUser.Text,
                                                          txtPassword.Text,
                                                          txtPaySubmitToken.Text,
                                                          decimal.Parse(txtSubmitResponseFee.Text),
                                                          int.Parse(txtSubmitResponseFilingID.Text),
                                                          txtURL.Text);

                if (response)
                {

                    GSCCCA.eFile.Integration.IacaStatus.v3.StatusDocument iacaStatus = await CallGetStatus.ServiceAsync(txtUser.Text,
                                                                      txtPassword.Text,
                                                                      int.Parse(txtSubmitResponseFilingID.Text),
                                                                      txtURL.Text);

                    txtSubmitResponseCallDate.Text = iacaStatus.Header.Date.Text.First();
                    txtSubmitResponseFilingID.Text = iacaStatus.Record.First().DocumentReceiptID.Text.First();
                    txtSubmitResponseFee.Text = iacaStatus.Record.First().EstimatedFee.Text.First();
                    txtSubmitResponseStausDate.Text = iacaStatus.Record.First().StatusDate.Text.First();
                    txtSubmitResponseStatus.Text = iacaStatus.Record.First().Status.value.ToString();
                }

                MessageBox.Show("Pay() called");

            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnGetStatusFilingID_Click(object sender, EventArgs e)
        {
            try
            {
                GSCCCA.eFile.Integration.IacaStatus.v3.StatusDocument iacaStatus = await CallGetStatus.ServiceAsync(txtUser.Text,
                                                                  txtPassword.Text,
                                                                  int.Parse(txtGetStatusFilingID.Text),
                                                                  txtURL.Text);

                txtSubmitResponseCallDate.Text = iacaStatus.Header.Date.Text.First();
                txtSubmitResponseFilingID.Text = iacaStatus.Record.First().DocumentReceiptID.Text.First();
                txtSubmitResponseFee.Text = iacaStatus.Record.First().EstimatedFee.Text.First();
                txtSubmitResponseStausDate.Text = iacaStatus.Record.First().StatusDate.Text.First();
                txtSubmitResponseStatus.Text = iacaStatus.Record.First().Status.value.ToString();

                MessageBox.Show("GetStatus() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnDownloadFileXml_Click(object sender, EventArgs e)
        {
            try
            {
                var iacaFile = await CallGetFilingData.ServiceAsync(txtUser.Text,
                                                                    txtPassword.Text,
                                                                    int.Parse(txtSubmitResponseFilingID.Text),
                                                                    txtURL.Text);

                SaveFileDialog saveDiag = new SaveFileDialog();
                saveDiag.Filter = "XML|*.xml";
                saveDiag.Title = "Save downloaded IACA File XML";
                saveDiag.ShowDialog();
                File.WriteAllText(saveDiag.FileName, XmlFormatter<GSCCCA.eFile.Integration.IacaFile.v3.FileDocument>.Serialize(iacaFile));

                MessageBox.Show("GetFilingData() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnDownloadFilePdf_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] document = await CallGetFilingDocument.ServiceAsync(txtUser.Text,
                                                                        txtPassword.Text,
                                                                        int.Parse(txtSubmitResponseFilingID.Text),
                                                                        txtURL.Text);

                SaveFileDialog saveDiag = new SaveFileDialog();
                saveDiag.Filter = "PDF|*.pdf";
                saveDiag.Title = "Save downloaded PDF Document";
                saveDiag.ShowDialog();
                File.WriteAllBytes(saveDiag.FileName, document);

                MessageBox.Show("GetFilingDocument() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnRefreshUCCStatus_Click(object sender, EventArgs e)
        {
            try
            {
                GSCCCA.eFile.Integration.IacaStatus.v3.StatusDocument iacaStatus = await CallGetStatus.ServiceAsync(txtUser.Text,
                                                                  txtPassword.Text,
                                                                  int.Parse(txtSubmitResponseFilingID.Text),
                                                                  txtURL.Text);

                txtSubmitResponseCallDate.Text = iacaStatus.Header.Date.Text.First();
                txtSubmitResponseFilingID.Text = iacaStatus.Record.First().DocumentReceiptID.Text.First();
                txtSubmitResponseFee.Text = iacaStatus.Record.First().EstimatedFee.Text.First();
                txtSubmitResponseStausDate.Text = iacaStatus.Record.First().StatusDate.Text.First();
                txtSubmitResponseStatus.Text = iacaStatus.Record.First().Status.value.ToString();

                MessageBox.Show("GetStatus() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }
        #endregion

        #region County Workflow Tabs
        private async void BtnLoadQueue_Click(object sender, EventArgs e)
        {
            try
            {
                listQueue.DataSource = await CallGetQueue.ServiceAsync(txtUser.Text, txtPassword.Text, txtURL.Text);

                MessageBox.Show("GetQueue() called");

                PopulateRejectionReasons();
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void ListQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox queue = sender as ListBox;
            if (queue == null)
                return;

            try
            {
                int filingID = int.Parse(queue.SelectedValue.ToString());

                var uccObject = await CallGetFilingData.ServiceAsync(txtUser.Text, txtPassword.Text, filingID, txtURL.Text);

                txtCountyWorkingUCCType.Text = uccObject.Record.FirstOrDefault()?.TransType.Type.ToString() ?? "n/a";
                txtCountyWorkingFilingID.Text = uccObject.Record.FirstOrDefault()?.DocumentReceiptID?.Text.FirstOrDefault() ?? "n/a";
                txtCountyWorkingFiler.Text = uccObject.Header.Filer.ContactName.Text.FirstOrDefault() ?? "n/a";
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private void CbAssignFileNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox fileNumberCheck))
                return;

            txtAcceptFileNumber.Enabled = fileNumberCheck.Checked;

            if (!txtAcceptFileNumber.Enabled)
                txtAcceptFileNumber.Text = string.Empty;
        }

        private void CbBackDate_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox backDateCheck))
                return;

            txtAcceptBackDate.Enabled = backDateCheck.Checked;

            if (!txtAcceptBackDate.Enabled)
                txtAcceptBackDate.Text = string.Empty;
        }

        private async void BtnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string endpoint = txtURL.Text;
                string user = txtUser.Text;
                string pass = txtPassword.Text;
                int filingID = int.Parse(txtCountyWorkingFilingID.Text);

                string fileNumber = (string.IsNullOrEmpty(txtAcceptFileNumber.Text)) ? "" : txtAcceptFileNumber.Text;
                DateTime backdate = (string.IsNullOrEmpty(txtAcceptBackDate.Text)) ? default : DateTime.Parse(txtAcceptBackDate.Text);


                var response = await CallAccept.ServiceAsync(user, pass, filingID, endpoint, fileNumber, backdate);

                listQueue.DataSource = await CallGetQueue.ServiceAsync(txtUser.Text, txtPassword.Text, txtURL.Text);

                MessageBox.Show("Accept() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void BtnReject_Click(object sender, EventArgs e)
        {
            try
            {
                var rejectionReasons = new List<string>();

                if (!string.IsNullOrEmpty(cbRejectReason.Text))
                    rejectionReasons.Add(cbRejectReason.Text);

                if (!string.IsNullOrEmpty(txtRejectReason.Text))
                    rejectionReasons.Add(txtRejectReason.Text);

                var response = await CallReject.ServiceAsync(txtUser.Text,
                                                             txtPassword.Text,
                                                             int.Parse(txtCountyWorkingFilingID.Text),
                                                             rejectionReasons,
                                                             txtURL.Text);

                listQueue.DataSource = await CallGetQueue.ServiceAsync(txtUser.Text, txtPassword.Text, txtURL.Text);

                MessageBox.Show("Reject() called");
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }

        private async void PopulateRejectionReasons()
        {
            try
            {
                cbRejectReason.DataSource = await CallGetRejectionReasons.ServiceAsync(txtURL.Text);
            }
            catch (SoapException spex)
            {
                DisplayErrorMessage(spex);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex);
            }
        }
        #endregion

        #region Display Methods
        private void DisplayErrorMessage(Exception ex)
        {
            if (ex is SoapException soap)
            {
                DisplayErrorMessage(soap);
                return;
            }

            if (ex is FaultException fault)
            {
                DisplayErrorMessage(fault);
                return;
            }

            string msg = string.Empty;
            msg += ($"CLIENT EXCEPTION CAUGHT\n\r");
            msg += ($"   At: {ex.Source}\n\r");
            msg += ($"   Data: {ex.Data}\n\r");
            msg += ($"   Message: {ex.Message}\n\r");
            MessageBox.Show(msg, "Error");
        }
        private void DisplayErrorMessage(SoapException ex)
        {
            string msg = string.Empty;
            msg += ("SERVER FAULT FOUND\n\r");
            msg += ($"   At: {ex.Source}\n\r");
            msg += ($"   Actor: {ex.Actor}\n\r");
            msg += ($"   Detail: {ex.Detail.Value}\n\r");
            msg += ($"   Code: {ex.Code.Name}\n\r");
            msg += ($"   Data: {ex.Data.Count}\n\r");
            msg += ($"   Message: {ex.Message}");
            MessageBox.Show(msg, "Soap Fault");
        }
        private void DisplayErrorMessage(FaultException ex)
        {
            string msg = string.Empty;
            msg += ("SERVER FAULT FOUND\n\r");
            msg += ($"   At: {ex.Source}\n\r");
            msg += ($"   Action: {ex.Action}\n\r");
            msg += ($"   Code: {ex.Code.Name}\n\r");
            msg += ($"   Data: {ex.Data.Count}\n\r");
            msg += ($"   Message: {ex.Message}");
            MessageBox.Show(msg, "Soap Fault");
        }
        #endregion
    }
}
