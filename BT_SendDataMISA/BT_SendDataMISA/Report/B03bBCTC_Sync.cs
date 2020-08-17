using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.B03;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class B03bBCTC_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public B03bBCTC_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<B03bBCTCModel> oListB02BCQT)
        {
            oListB02BCQT = new List<B03bBCTCModel>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string pStartDate = _dbMisaInfo.StartDate;
                    string pFromDate = eachMonth.FromDate;
                    string pToDate = eachMonth.ToDate;
                    string pBudgetChapter = null;
                    int pSummaryBudgetChapter = 0;
                    string pMasterID = null;

                    string msg = Exec.MultipleResult("Proc_FIR_Get03b_BCTC_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetChapter, pSummaryBudgetChapter, pMasterID }, out ReportHeader outItem, out List<B03bBCTCDetailItem> oList);
                    if (msg.Length > 0) return Msg.Exec_Proc_FIR_Get03b_BCTC_ExportForX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int BudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "B03bBCTC";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = BudgetChapterCode;

                        B03bBCTCModel b03bBCTC = new B03bBCTCModel
                        {
                            ReportHeader = outItem,
                            B03bBCTCDetail = oList
                        };

                        oListB02BCQT.Add(b03bBCTC);
                    }
                }
            }
            if (oListB02BCQT.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<B03bBCTCModel> oListB03bBCTC);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:B03bBCTC_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:B02BCTC_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListB03bBCTC);
        }
    }
}
