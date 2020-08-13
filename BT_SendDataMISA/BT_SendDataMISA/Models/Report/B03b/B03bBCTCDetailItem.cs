using System;

namespace BT_SendDataMISA.Models.Report.B03
{
    public class B03bBCTCDetailItem
    {
        public Guid RefID { get; set; }
        public string ReportItemName { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemAlias { get; set; }
        public int ReportItemIndex { get; set; }
        public string ReportItemDescription { get; set; }
        public decimal PrevAmount { get; set; }
        public decimal Amount { get; set; }

        public string BudgetKindItemID { get; set; }
        public string BudgetSubKindItemID { get; set; }
    }
}
