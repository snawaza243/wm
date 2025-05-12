using System;

namespace WM.Models
{
    public class DueAndPaidModel
    {

        public string CompanyCd { get; set; }
        public string StatusCd { get; set; }
        public string Location { get; set; }
        public string PolicyNo { get; set; }
        public string ClName { get; set; }
        public decimal? PremAmt { get; set; }
        public string PayMode { get; set; }
        public DateTime? DueDate { get; set; }
        public string SA { get; set; }
        public string ClAdd1 { get; set; }
        public string ClAdd2 { get; set; }
        public string ClAdd3 { get; set; }
        public string ClAdd4 { get; set; }
        public string ClAdd5 { get; set; }
        public string ClCity { get; set; }
        public string ClPin { get; set; }
        public string ClPhone1 { get; set; }
        public string ClPhone2 { get; set; }
        public string ClMobile { get; set; }
        public string PlanName { get; set; }
        public string PremFreq { get; set; }
        public string Doc { get; set; }
        public string PlyTerm { get; set; }
        public DateTime? EcsDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? NetAmount { get; set; }
        public DateTime? FupDate { get; set; }
        public string Ppt { get; set; }
        public int? MonNo { get; set; }
        public int? YearNo { get; set; }
        public string Userid { get; set; }
        public DateTime? ImportDt { get; set; }
        public string ImportDataType { get; set; }
        public int? NewInsert { get; set; }
        public int? DuePaid { get; set; }
    }

}