using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.F01_02
{
    public class F01_02_P2BCQTModel
    {
        public F01_02_P2BCQTModel()
        {
            F01_02_P2BCQTDetail = new List<F01_02_P2BCQTDetailItem>();
            F01_02_P2BCQTProject = new List<F01_02_P2BCQTProjectItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<F01_02_P2BCQTDetailItem> F01_02_P2BCQTDetail { get; set; }
        public List<F01_02_P2BCQTProjectItem> F01_02_P2BCQTProject { get; set; }
    }
}
