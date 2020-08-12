using System;

namespace BT_SendDataMISA.Models
{
    public class DbMisaInfo
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int ExportVersion { get; set; }
        public int ParticularID { get; set; }
        public string ProductID { get; set; }
        public string Version { get; set; }
        public string BudgetChapterID { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetSubKindItemID { get; set; }
        public string StartDate { get; set; }
        public string AccountSystem { get; set; }

        public string ExportDate { get; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        public string RefID { get; set; }
        public string ReportID { get; set; }
        public int ReportPeriod { get; set; }
        public int ReportYear { get; set; }
    }
}
