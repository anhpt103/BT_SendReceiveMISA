using System;

namespace BT_SendDataMISA.Models.Report
{
    public class B02BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public decimal PreviousPeriodAmount { get; set; }
        public decimal CurrentPeriodAmount { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetSourceID { get; set; }
        public string BudgetSubKindItemID { get; set; }
        public string ReportItemAlias { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemIndex { get; set; }
        public string ReportItemName { get; set; }
    }
}
