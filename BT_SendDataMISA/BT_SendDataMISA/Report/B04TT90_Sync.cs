using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.B04TT90;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class B04TT90_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public B04TT90_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<B04TT90Model> oListB04TT90)
        {
            oListB04TT90 = new List<B04TT90Model>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string StartDate = _dbMisaInfo.StartDate;
                    string FromDate = eachMonth.FromDate;
                    string ToDate = eachMonth.ToDate;
                    string BudgetChapterCode = null;
                    int IsSummaryChapter = 0;

                    string msg = Exec.MultipleResult("Proc_Other_GetB04_TT902018_ExportForX1", new { StartDate, FromDate, ToDate, BudgetChapterCode, IsSummaryChapter }, out ReportHeader outItem, out List<B04TT90DetailItem> oList);
                    if (msg.Length > 0) return Msg.Exec_Proc_Other_GetB04_TT902018_ExportForX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int oBudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "B04TT90";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = oBudgetChapterCode;
                        outItem.BudgetChapterID = oBudgetChapterCode;

                        B04TT90Model b04TT90 = new B04TT90Model
                        {
                            ReportHeader = outItem,
                            B04TT90Detail = oList
                        };

                        oListB04TT90.Add(b04TT90);
                    }
                }
            }
            if (oListB04TT90.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<B04TT90Model> oListB04TT90);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:B04TT90_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:B04TT90_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListB04TT90);
        }
    }
}
