using System;

namespace BT_SendDataMISA.Models.Report.F01_02
{
    public class F01_02_P1BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public string ProjectID { get; set; }
        public int MethodDistributeID { get; set; }
        public string BudgetSourceID { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetSubKindItemID { get; set; }
        public string ReportItemAlias { get; set; }
        public string ReportItemName { get; set; }
        public int ReportItemIndex { get; set; }
        public string ReportItemCode { get; set; }
        public decimal TotalAmountApproved { get; set; }
        public decimal Amount { get; set; }
        public decimal AccAmount { get; set; }
    }
}
