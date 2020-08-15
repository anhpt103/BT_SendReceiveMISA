using System;

namespace BT_SendDataMISA.Models.Report.B01
{
    public class B01BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public string ReportItemName { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemAlias { get; set; }
        public int ReportItemIndex { get; set; }
        public decimal Amount { get; set; }

        public string BudgetSourceID { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetSubKindItemID { get; set; }
    }
}
