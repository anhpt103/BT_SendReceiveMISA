using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.B02;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BT_SendDataMISA.Report
{
    public class B02BCTC_Sync
    {
        private string _startDate;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        public B02BCTC_Sync(string startDate, string urlAPI, string token, IConfiguration configuration)
        {
            _startDate = startDate;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
        }

        private string GetDataReport(out List<B02BCQT> oListB02BCQT)
        {
            oListB02BCQT = new List<B02BCQT>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string pStartDate = _startDate;
                    string pFromDate = eachMonth.FromDate;
                    string pToDate = eachMonth.ToDate;
                    string pBudgetChapter = null;
                    int pSummaryBudgetChapter = 0;
                    string pMasterID = null;
                    string @pIsPrintMonth13 = null;

                    string msg = Exec.MultipleResult("Proc_FIR_Get02_BCTC_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetChapter, pSummaryBudgetChapter, pMasterID, @pIsPrintMonth13 }, out ReportHeader outItem, out List<B02BCQTDetailItem> oList);
                    if (msg.Length > 0) return "Xảy ra lỗi khi Exec: Proc_FIR_Get02_BCTC_ExportForX1";
                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        B02BCQT b02BCQT = new B02BCQT
                        {
                            ReportHeader = outItem,
                            B02BCTCDetail = oList
                        };

                        oListB02BCQT.Add(b02BCQT);
                    }
                }
            }
            if (oListB02BCQT.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public string SendDataToAPI()
        {
            string msg = GetDataReport(out List<B02BCQT> oListB02BCQT);
            if (msg.Length > 0) return msg;

            string api = _configuration.GetValue<string>("ApiName:B02BCQT_Receive");
            if (string.IsNullOrEmpty(api)) return "Không tìm thấy cấu hình ApiName:B02BCQT_Receive trong file appsettings.json";

            HttpClientPost httpClientPost = new HttpClientPost();
            httpClientPost.SendsRequestWithToken(_urlAPI + api, _token, oListB02BCQT);

            return "";
        }
    }
}
