using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.B03TT90;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class B03TT90_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public B03TT90_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<B03TT90Model> oListB03TT90)
        {
            oListB03TT90 = new List<B03TT90Model>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string pStartDate = _dbMisaInfo.StartDate;
                    string pFromDate = eachMonth.FromDate;
                    string pToDate = eachMonth.ToDate;
                    string BudgetChapterCode = null;
                    int IsSummaryChapter = 0;

                    string msg = Exec.MultipleResult("Proc_Other_GetB03_TT902018_ExportForX1", new { pStartDate, pFromDate, pToDate, BudgetChapterCode, IsSummaryChapter }, out ReportHeader outItem, out List<B03TT90DetailItem> oList);
                    if (msg.Length > 0) return Msg.Exec_Proc_FIR_Get02_BCTC_ExportForX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int oBudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "B03TT90";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = oBudgetChapterCode;
                        outItem.BudgetChapterID = oBudgetChapterCode;

                        B03TT90Model b01BCTC = new B03TT90Model
                        {
                            ReportHeader = outItem,
                            B03TT90Detail = oList
                        };

                        oListB03TT90.Add(b01BCTC);
                    }
                }
            }
            if (oListB03TT90.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<B03TT90Model> oListB03TT90);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:B03TT90_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:B03TT90_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListB03TT90);
        }
    }
}
