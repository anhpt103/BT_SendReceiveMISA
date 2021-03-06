﻿using AutoMapper;
using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Report;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BT_SendDataMISA.Models.AccessToken;

namespace BT_SendDataMISA
{
    public class Worker : BackgroundService
    {
        private string TenUngDung = "";
        private readonly ILogger<Worker> _logger;
        private readonly IMapper _mapper;
        public IConfiguration _configuration { get; }

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped...");
            return base.StopAsync(cancellationToken);
        }

        StringBuilder sb = new StringBuilder(string.Format(@"Gửi dữ liệu Start: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
        private StringBuilder WriteLog(string msg)
        {
            _logger.LogError(msg);
            sb.Append(msg);
            return sb;
        }

        private async Task<ParamSetting> GetParamSettingFromServer(string urlAPI)
        {
            Result result = await GetAccessTokenAsync();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog(reasons.FirstOrDefault().Message);
                return null;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out TokenInfo accessToken);
            if (msg.Length > 0)
            {
                WriteLog(Msg.Convert_TokenInfo_Err);
                return null;
            }

            // Get SysScheduler From Root Database
            result = await GetSysScheduler(urlAPI, accessToken.access_token);
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog(reasons.FirstOrDefault().Message);
                return null;
            }

            successes = result.Successes;
            Convertor.StringToObject(successes.FirstOrDefault().Message, out List<SysScheduler> sysScheduler);
            if (msg.Length > 0)
            {
                WriteLog(Msg.Convert_Lst_SysScheduler_Err);
                return null;
            }

            ParamSetting paramSetting = new ParamSetting
            {
                TokenInfos = accessToken,
                ListSysScheduler = sysScheduler
            };

            return paramSetting;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TenUngDung = _configuration.GetValue<string>("exeConfigFile:TenUngDung");
            if (string.IsNullOrEmpty(TenUngDung)) { WriteLog(Msg.TenUngDung_AppST_404); return; }

            string urlAPI = _configuration.GetValue<string>("WebServer:UrlAPI");
            if (string.IsNullOrEmpty(urlAPI)) { WriteLog(Msg.UrlAPI_AppST_404); return; }

            string msg = DoBeginProcessSync();
            if (msg.Length > 0) WriteLog(msg);

            // Get DBInfo Misa
            msg = CommonFunction.GetDBInfoMisa(out DbMisaInfo oMisaInfo);
            if (msg.Length > 0) WriteLog(msg);

            while (!stoppingToken.IsCancellationRequested)
            {
                ParamSetting paramSetting = await GetParamSettingFromServer(urlAPI);
                if (paramSetting == null) WriteLog("Lỗi GetParamSettingFromServer");

                var application = paramSetting.ListSysScheduler.FirstOrDefault(x => x.TEN_UNGDUNG == TenUngDung);
                if (application != null && application.TEN_UNGDUNG == TenUngDung)
                {
                    DoWork(oMisaInfo, urlAPI, paramSetting.TokenInfos.access_token);
                }

                await Task.Delay((60000 * application.TIME_PERIOD), stoppingToken);
            }
        }

        private void DoWork(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            DoWorkB01BCTC(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB02BCTC(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB03bBCTC(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB04BCTC(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB01BCQT(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB03TT90(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkB04TT90(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkF01_01BCQT(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkF01_02_P1BCQT(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
            DoWorkF01_02_P2BCQT(oMisaInfo, urlAPI, access_token);
            WriteLog("-------------------------------------");
        }

        private async void DoWorkB01BCTC(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B01BCTC <--");
            B01BCTC_Sync b01BCTC_Sync = new B01BCTC_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b01BCTC_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B01BCTC:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B01BCTC" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B01BCTC <--");
        }

        private async void DoWorkB02BCTC(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B02BCTC <--");
            B02BCTC_Sync b02BCTC_Sync = new B02BCTC_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b02BCTC_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B02BCTC:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B02BCTC" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B02BCTC <--");
        }

        private async void DoWorkB03bBCTC(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B03bBCTC <--");
            B03bBCTC_Sync b03bBCTC_Sync = new B03bBCTC_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b03bBCTC_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B03bBCTC:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B03bBCTC" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B03bBCTC <--");
        }

        private async void DoWorkB04BCTC(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B04BCTC <--");
            B04BCTC_Sync b04BCTC_Sync = new B04BCTC_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b04BCTC_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B04BCTC:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B04BCTC" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B04BCTC <--");
        }

        private async void DoWorkB01BCQT(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B01BCQT <--");
            B01BCQT_Sync b01BCQT_Sync = new B01BCQT_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b01BCQT_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B01BCQT:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B01BCQT" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B01BCQT <--");
        }

        private async void DoWorkB03TT90(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B03TT90 <--");
            B03TT90_Sync b01TT90_Sync = new B03TT90_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b01TT90_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B03TT90:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B03TT90" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B03TT90 <--");
        }

        private async void DoWorkB04TT90(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo B04TT90 <--");
            B04TT90_Sync b04TT90_Sync = new B04TT90_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await b04TT90_Sync.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo B04TT90:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo B04TT90" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo B04TT90 <--");
        }

        private async void DoWorkF01_01BCQT(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo F01_01BCQT <--");
            F01_01BCQT_Sync f01_01BCQT = new F01_01BCQT_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await f01_01BCQT.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo F01_01BCQT:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo F01_01BCQT" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo F01_01BCQT <--");
        }

        private async void DoWorkF01_02_P1BCQT(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo F01_02_P1BCQT <--");
            F01_02_P1BCQT_Sync f01_02_P1BCQT = new F01_02_P1BCQT_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await f01_02_P1BCQT.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo F01_02_P1BCQT:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo F01_02_P1BCQT" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo F01_02_P1BCQT <--");
        }

        private async void DoWorkF01_02_P2BCQT(DbMisaInfo oMisaInfo, string urlAPI, string access_token)
        {
            WriteLog("--> Bắt đầu gửi dữ liệu báo cáo F01_02_P2BCQT <--");
            F01_02_P2BCQT_Sync f01_02_P2BCQT = new F01_02_P2BCQT_Sync(oMisaInfo, urlAPI, access_token, _configuration, _mapper);
            Result result = await f01_02_P2BCQT.SendDataToAPI();
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                WriteLog("Xảy ra lỗi gửi dữ liệu báo cáo F01_02_P2BCQT:");
                WriteLog(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            string msg = Convertor.StringToObject(successes.FirstOrDefault().Message, out ResponseObj resObj);
            if (msg.Length > 0) WriteLog(Msg.Convert_ResponseObj_Err);
            WriteLog(string.IsNullOrEmpty(resObj.Message) ? "Gửi dữ liệu thành công báo cáo F01_02_P2BCQT" : resObj.Message);
            WriteLog("--> Kết thúc gửi dữ liệu báo cáo F01_02_P2BCQT <--");
        }

        private async Task<Result> GetAccessTokenAsync()
        {
            string apiGetToken = _configuration.GetValue<string>("AuthenToken:api");
            if (string.IsNullOrEmpty(apiGetToken)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:api trong file appsettings.json");

            string username = _configuration.GetValue<string>("AuthenToken:username");
            if (string.IsNullOrEmpty(username)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:username trong file appsettings.json");

            string password = _configuration.GetValue<string>("AuthenToken:password");
            if (string.IsNullOrEmpty(password)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:password trong file appsettings.json");

            string client_id = _configuration.GetValue<string>("AuthenToken:client_id");
            if (string.IsNullOrEmpty(client_id)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:client_id trong file appsettings.json");

            string grant_type = _configuration.GetValue<string>("AuthenToken:grant_type");
            if (string.IsNullOrEmpty(grant_type)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:grant_type trong file appsettings.json");

            string tendiaban = _configuration.GetValue<string>("AuthenToken:tendiaban");
            if (string.IsNullOrEmpty(tendiaban)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:tendiaban trong file appsettings.json");

            TokenParam tokenParam = new TokenParam
            {
                username = username,
                password = password,
                client_id = client_id,
                grant_type = grant_type,
                tendiaban = tendiaban
            };

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(apiGetToken, "", tokenParam);
        }

        private async Task<Result> GetSysScheduler(string urlAPI, string token)
        {
            string api = _configuration.GetValue<string>("ApiName:GetScheduler");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:GetScheduler trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequest(urlAPI + api, token);
        }

        private string DoBeginProcessSync()
        {
            ReadConfigTextFile readConfigTextFile = new ReadConfigTextFile(_configuration);
            string msg = readConfigTextFile.ReadFile(out string outStr);
            if (msg.Length > 0 && string.IsNullOrEmpty(outStr))
            {
                AccessibleFiles accessibleFiles = new AccessibleFiles(_logger);

                string folder = _configuration.GetValue<string>("exeConfigFile:Directory");
                string fileName = _configuration.GetValue<string>("exeConfigFile:Name");
                if (string.IsNullOrEmpty(folder)) return "Không tìm thấy cấu hình exeConfigFile:Directory trong file appsettings.json";
                if (string.IsNullOrEmpty(fileName)) return "Không tìm thấy cấu hình exeConfigFile:Name trong file appsettings.json";

                IEnumerable<string> pathDirec = accessibleFiles.SearchAccessibleFiles(folder, fileName);
                if (pathDirec.Count() == 0) return "Kết thúc gửi dữ liệu do không tìm thấy file Misa Config...";

                WriteConfigTextFile writeConfigTextFile = new WriteConfigTextFile(_configuration);
                string pathConfig = pathDirec.FirstOrDefault();
                msg = writeConfigTextFile.WriteToFile(pathConfig, out outStr);
                if (msg.Length > 0 || string.IsNullOrEmpty(outStr)) return msg;
            }

            GetConnecStringInFile getConnecString = new GetConnecStringInFile(_configuration);
            msg = getConnecString.GetStringConnectDatabase(outStr, out string connectString);
            if (msg.Length > 0) return msg;

            var resultConn = Exec.GetDbConnection(connectString);
            if (resultConn == null) return "Xảy ra lỗi khi kết nối Cơ sở dữ liệu";

            return "";
        }
    }
}
