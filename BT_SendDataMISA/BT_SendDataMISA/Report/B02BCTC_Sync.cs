using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.Models.Report;
using System.Collections.Generic;

namespace BT_SendDataMISA.Report
{
    public class B02BCTC_Sync
    {
        public string GetDataReport(string startDate)
        {
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string pStartDate = startDate;
                    string pFromDate = eachMonth.FromDate;
                    string pToDate = eachMonth.ToDate;
                    string pBudgetChapter = null;
                    int pSummaryBudgetChapter = 0;
                    string pMasterID = null;
                    string @pIsPrintMonth13 = null;

                    string msg = Exec.MultipleResult("Proc_FIR_Get02_BCTC_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetChapter, pSummaryBudgetChapter, pMasterID, @pIsPrintMonth13 }, out ReportHeader outItem, out List<B02BCQTDetailItem> oList);
                    if (outItem != null && (oList != null && oList.Count > 0))
                    {

                    }
                }
            }

            return "";
        }
    }
}
