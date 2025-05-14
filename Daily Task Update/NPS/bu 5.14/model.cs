using System;
using System.ComponentModel.DataAnnotations;

namespace WM.Masters
{
    public class NpsTransactionPunchingModel
    {
        // Original Parameters
        public int Index { get; set; }
        public string GlbroleId { get; set; }
        public string TxtdocID { get; set; }
        public int OptCorporate { get; set; }
        public string Txtcorporatename { get; set; }
        public int ChkUnfreeze { get; set; }
        public string TxtPRAN { get; set; }
        public string TxtINV_CD { get; set; }
        public string TxtChqNo { get; set; }
        public decimal TxtAmountInvest { get; set; }
        public DateTime DtDate { get; set; }
        public string CmbBankName { get; set; }
        public string Txtregistrationno { get; set; }
        public string Cmbproduct { get; set; }
        public string ReqCode { get; set; }
        public string MutCode { get; set; }
        public string SCHCODE { get; set; }
        public string Glbloginid { get; set; }
        public string ClientBranchCode { get; set; }
        public string ClientRmCode { get; set; }
        public string Busi_Branch_cd { get; set; }
        public string Busi_Rm_Cd { get; set; }

        // Additional Parameters
        public string Amount { get; set; }
        public string ClientCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string DocID { get; set; }
        public string SchemeCode { get; set; }
        public string RequestCode { get; set; }
        public string InvestmentType { get; set; }
        public string UserID { get; set; }
        public string BussinessBranch { get; set; }
        public string DTNumber { get; set; }
        public string InvCode { get; set; }
        public string InvName { get; set; }
        public string BusiRmCode { get; set; }
        public string BranchCode { get; set; }
        public string Exitsr { get; set; }
        public string Pran { get; set; }
        public string InvestorName { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public string EcsDate { get; set; }
        public string BankName { get; set; }
        public string UniqueId { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal RegCharge { get; set; }
        public decimal TranCharge { get; set; }
        public decimal Service { get; set; }
        public decimal AmtInv { get; set; }
        public string ReceiptNo10To17 { get; set; }
        public int? FcRegistrationNo { get; set; }
        public string Remarks { get; set; }
        public string Remarks1 { get; set; }
        public string TranCode { get; set; }
        public string RefTranCode { get; set; }
        public string CsfTransactionId { get; set; }


    }
} 