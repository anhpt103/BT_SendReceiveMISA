using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.F01_02;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class F01_02_P2BCQT_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public F01_02_P2BCQT_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<F01_02_P2BCQTModel> oListF01_02_P2BCQT)
        {
            oListF01_02_P2BCQT = new List<F01_02_P2BCQTModel>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string StartDate = _dbMisaInfo.StartDate;
                    string FromDate = eachMonth.FromDate;
                    string ToDate = eachMonth.ToDate;
                    string ListBudgetSourceID = null;
                    string BudgetChapterCode = null;
                    string ListBudgetKindItemCode = null;
                    string EnumMethodDistributeID = null;
                    string ProjectID = null;
                    int IsSummaryBudgetSource = 0;
                    int IsSummaryBudgetChapter = 0;
                    int IsSummaryBudgetKindItem = 0;
                    int IsSummaryMethodDistribute = 0;
                    int IsSummaryProject = 0;

                    string msg = Exec.ThirdOutputResult("Proc_FIR_GetF01_02_BCQT_P2_ToX1", new { StartDate, FromDate, ToDate, ListBudgetSourceID, BudgetChapterCode, ListBudgetKindItemCode, EnumMethodDistributeID, ProjectID, IsSummaryBudgetSource, IsSummaryBudgetChapter, IsSummaryBudgetKindItem, IsSummaryMethodDistribute, IsSummaryProject }, out ReportHeader outItem, out List<F01_02_P2BCQTDetailItem> oList, out List<F01_02_P2BCQTProjectItem> oListProject);
                    if (msg.Length > 0) return Msg.Exec_Proc_FIR_GetF01_02_BCQT_P1_ToX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int oBudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "F01_02_P2_BCQT";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = oBudgetChapterCode;
                        outItem.BudgetChapterID = oBudgetChapterCode;

                        F01_02_P2BCQTModel f01_02_P2BCQT = new F01_02_P2BCQTModel
                        {
                            ReportHeader = outItem,
                            F01_02_P2BCQTDetail = oList,
                            F01_02_P2BCQTProject = oListProject
                        };

                        oListF01_02_P2BCQT.Add(f01_02_P2BCQT);
                    }
                }
            }
            if (oListF01_02_P2BCQT.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<F01_02_P2BCQTModel> oListF01_02_P2BCQT);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:F01_02_P2BCQT_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:F01_02_P2BCQT_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListF01_02_P2BCQT);
        }
    }
}
