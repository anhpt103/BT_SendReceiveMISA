using System.Collections.Generic;

namespace BT_SendDataMISA.Models.Report.B04TT90
{
    public class B04TT90Model
    {
        public B04TT90Model()
        {
            B04TT90Detail = new List<B04TT90DetailItem>();
        }
        public ReportHeader ReportHeader { get; set; }
        public List<B04TT90DetailItem> B04TT90Detail { get; set; }
    }
}
