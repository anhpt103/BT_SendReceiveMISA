using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.B01
{
    public class B01BCQTModel
    {
        public B01BCQTModel()
        {
            B01BCQTDetail = new List<B01BCQTDetailItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<B01BCQTDetailItem> B01BCQTDetail { get; set; }
    }
}
