﻿using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;
using BT_SendDataMISA.Models.Report.B01;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_SendDataMISA.Report
{
    public class B01BCQT_Sync
    {
        private DbMisaInfo _dbMisaInfo;
        private string _urlAPI;
        private string _token;
        public IConfiguration _configuration { get; }
        private readonly IMapper _mapper;

        public B01BCQT_Sync(DbMisaInfo dbMisaInfo, string urlAPI, string token, IConfiguration configuration, IMapper mapper)
        {
            _dbMisaInfo = dbMisaInfo;
            _urlAPI = urlAPI;
            _token = token;
            _configuration = configuration;
            _mapper = mapper;
        }

        private string GetDataReport(out List<B01BCQTModel> oListB01BCQT)
        {
            oListB01BCQT = new List<B01BCQTModel>();
            var listStartEndDateOYear = CommonFunction.GetStartEndDateAllMonthInYear();
            if (listStartEndDateOYear.Count > 0)
            {
                foreach (var eachMonth in listStartEndDateOYear)
                {
                    string StartDate = _dbMisaInfo.StartDate;
                    string FromDate = eachMonth.FromDate;
                    string ToDate = eachMonth.ToDate;
                    int IsSummarySXKD = 0;

                    string msg = Exec.MultipleResult("Proc_FIR_Get01_BCQT_ToX1", new { StartDate, FromDate, ToDate, IsSummarySXKD }, out ReportHeader outItem, out List<B01BCQTDetailItem> oList);
                    if (msg.Length > 0) return Msg.Exec_Proc_FIR_Get01_BCQT_ToX1_Err;

                    if (outItem != null && (oList != null && oList.Count > 0))
                    {
                        Guid RefID = outItem.RefID;
                        int BudgetChapterCode = outItem.BudgetChapterCode;

                        outItem = _mapper.Map<ReportHeader>(_dbMisaInfo);
                        outItem.RefID = RefID;
                        outItem.ReportID = "B01BCQT";
                        outItem.ReportPeriod = eachMonth.Month;
                        outItem.ReportYear = eachMonth.Year;
                        outItem.BudgetChapterCode = BudgetChapterCode;
                        outItem.BudgetChapterID = BudgetChapterCode;

                        B01BCQTModel b01BCTC = new B01BCQTModel
                        {
                            ReportHeader = outItem,
                            B01BCQTDetail = oList
                        };

                        oListB01BCQT.Add(b01BCTC);
                    }
                }
            }
            if (oListB01BCQT.Count == 0) return "Không có dữ liệu báo cáo";

            return "";
        }

        public async Task<Result> SendDataToAPI()
        {
            string msg = GetDataReport(out List<B01BCQTModel> oListB01BCQT);
            if (msg.Length > 0) return Result.Fail(msg);

            string api = _configuration.GetValue<string>("ApiName:B01BCQT_Receive");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:B01BCTC_Receive trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(_urlAPI + api, _token, oListB01BCQT);
        }
    }
}
