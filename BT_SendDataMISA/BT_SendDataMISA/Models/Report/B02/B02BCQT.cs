using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.B02
{
    public class B02BCQT
    {
        public B02BCQT()
        {
            B02BCTCDetail = new List<B02BCQTDetailItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<B02BCQTDetailItem> B02BCTCDetail { get; set; }
    }
}
