using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.F01_01;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class F01_01BCQT_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public F01_01BCQT_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<F01_01BCQTModel> oListF01_01BCQT)
        {
            oListF01_01BCQT = new List<F01_01BCQTModel>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string pStartDate = _dbMisaInfo.StartDate;
                    string pFromDate = eachMonth.FromDate;
                    string pToDate = eachMonth.ToDate;
                    string pBudgetSource = null;
                    string pBudgetChapter = null;
                    string pBudgetSubKindItem = null;
                    int pSummaryBudgetSource = 0;
                    int pSummaryBudgetChapter = 0;
                    int pSummaryBudgetSubKindItem = 0;
                    int IsSummarySXKD = 0;

                    string msg = Exec.MultipleResult("Proc_FIR_Get01_01_BCQT_ExportForX1", new { pStartDate, pFromDate, pToDate, pBudgetSource, pBudgetChapter, pBudgetSubKindItem, pSummaryBudgetSource, pSummaryBudgetChapter, pSummaryBudgetSubKindItem, IsSummarySXKD }, out ReportHeader outItem, out List<F01_01BCQTDetailItem> oList);
                    if (msg.Length > 0) return Msg.Exec_Proc_FIR_Get01_01_BCQT_ExportForX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int oBudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "F01_01BCQT";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = oBudgetChapterCode;
                        outItem.BudgetChapterID = oBudgetChapterCode;

                        F01_01BCQTModel f01_01BCQT = new F01_01BCQTModel
                        {
                            ReportHeader = outItem,
                            F01_01BCQTDetail = oList
                        };

                        oListF01_01BCQT.Add(f01_01BCQT);
                    }
                }
            }
            if (oListF01_01BCQT.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<F01_01BCQTModel> oListF01_01BCQT);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:F01_01BCQT_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:F01_01BCQT_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListF01_01BCQT);
        }
    }
}
