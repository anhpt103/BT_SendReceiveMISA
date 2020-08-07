namespace BT_SendDataMISA.Models.Report
{
    public class B02BCTCModel
    {
        public string RefID { get; set; }
        public string ReportItemAlias { get; set; }
        public string ReportItemName { get; set; }
        public int ReportItemIndex { get; set; }
        public string ReportItemCode { get; set; }
        public string ReportItemDescription { get; set; }
        public decimal PreviousPeriodAmount { get; set; }
        public decimal CurrentPeriodAmount { get; set; }
    }
}
