using System;

namespace BT_SendDataMISA.Models.Report.B04TT90
{
    public class B04TT90DetailItem
    {
        public Guid RefID { get; set; }
        public string ReportItemName { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemAlias { get; set; }
        public int ReportItemIndex { get; set; }
        public decimal EstimateAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
    }
}
