using System;

namespace BT_SendDataMISA.Models.Report
{
    public class B02BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public int ReportItemIndex { get; set; }
        public string ReportItemAlias { get; set; }
        public string ReportItemName { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemDescription { get; set; }
        public decimal CurrentPeriodAmount { get; set; }
        public decimal PreviousPeriodAmount { get; set; }
    }
}
