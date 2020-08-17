using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.F01_02
{
    public class F01_02_P1BCQTModel
    {
        public F01_02_P1BCQTModel()
        {
            F01_02_P1BCQTDetail = new List<F01_02_P1BCQTDetailItem>();
            F01_02_P1BCQTProject = new List<F01_02_P1BCQTProjectItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<F01_02_P1BCQTDetailItem> F01_02_P1BCQTDetail { get; set; }
        public List<F01_02_P1BCQTProjectItem> F01_02_P1BCQTProject { get; set; }
    }
}
