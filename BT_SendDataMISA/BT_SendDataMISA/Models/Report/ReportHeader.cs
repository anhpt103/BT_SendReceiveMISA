using System;

namespace BT_SendDataMISA.Models.Report
{
    public class ReportHeader
    {
        public Guid RefID { get; set; }
        public string AccountSystem { get; set; }
        public int? BudgetChapterID { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string ExportDate { get; set; }
        public int? ExportVersion { get; set; }
        public int? ParticularID { get; set; }
        public string ProductID { get; set; }
        public string ReportID { get; set; }
        public int? ReportPeriod { get; set; }
        public int ReportYear { get; set; }
        public string Version { get; set; }
    }
}
