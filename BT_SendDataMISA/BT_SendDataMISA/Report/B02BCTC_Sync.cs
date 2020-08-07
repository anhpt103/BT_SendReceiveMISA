using BT_SendDataMISA.Common;
using BT_SendDataMISA.Models.Report;
using System.Collections.Generic;

namespace BT_SendDataMISA.Report
{
    public class B02BCTC_Sync
    {
        public string GetDataReport()
        {
            string pStartDate = "2020-01-01 00:00:00";
            string pFromDate = "2020-01-01 00:00:00";
            string pToDate = "2020-12-31 00:00:00";
            string pBudgetChapter = null;
            int pSummaryBudgetChapter = 0;
            string pMasterID = null;
            string @pIsPrintMonth13 = null;

            string msg = Exec.GetOne("Proc_FIR_Get02_BCTC_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetChapter, pSummaryBudgetChapter, pMasterID, @pIsPrintMonth13 }, out ReportHeader outItem);
            msg = Exec.GetList("Proc_FIR_Get02_BCTC_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetChapter, pSummaryBudgetChapter, pMasterID, @pIsPrintMonth13 }, out List<B02BCQTDetailItem> outListItem);
            return "";
        }
    }
}
