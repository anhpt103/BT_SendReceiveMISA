using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.B01
{
    public class B01BCTCModel
    {
        public B01BCTCModel()
        {
            B01BCTCDetail = new List<B01BCTCDetailItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<B01BCTCDetailItem> B01BCTCDetail { get; set; }
    }
}
