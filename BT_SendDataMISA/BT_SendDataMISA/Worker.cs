using BT_SendDataMISA.Common;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BT_SendDataMISA
{
    public class Worker : BackgroundService
    {
        private const string MISA_BAMBOO = "MISA Bamboo.NET";
        private const string MISA_MIMOSA = "MISA Mimosa.NET";

        private readonly ILogger<Worker> _logger;
        public IConfiguration _configuration { get; }

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string msg = DoBeginProcessSync();
            if (msg.Length > 0) _logger.LogError(msg);

            GetUnitSendData(out string MaDonViQuanHeNganSach, out string TenDonViQuanHeNganSach);

            while (!stoppingToken.IsCancellationRequested)
            {
                string urlAPI = _configuration.GetValue<string>("WebServer:UrlAPI");
                if (string.IsNullOrEmpty(urlAPI)) { _logger.LogError("Không tìm thấy cấu hình WebServer:UrlAPI trong file appsettings.json"); return; }

                msg = GetReport(out List<TT3442016_B07> outTT3442016_B07);
                if (msg.Length > 0) _logger.LogError(msg);

                HttpClientPost httpClientPost = new HttpClientPost();
                Result response = await httpClientPost.SendsRequest(urlAPI + "CauHinhDongBo/Post_TT3442016_B07", outTT3442016_B07);

                //var result = await httpClient.GetAsync("https://localhost:44390/api/SettingInfo/GetSetting/L3HGMGzlMl3ni2rluvBL1QqQZ6Ain2y5");
                //if (result.IsSuccessStatusCode)
                //{
                //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //}
                //else
                //{
                //    _logger.LogInformation("Xảy ra lỗi", result.StatusCode);
                //}

                await Task.Delay((60 * 1000 * 60), stoppingToken);
            }
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

        private string GetUnitSendData(out string MaDonViQuanHeNganSach, out string TenDonViQuanHeNganSach)
        {
            MaDonViQuanHeNganSach = TenDonViQuanHeNganSach = "";

            string exeConfigName = _configuration.GetValue<string>("exeConfigFile:Name");
            if (string.IsNullOrEmpty(exeConfigName)) return "Không tìm thấy cấu hình exeConfigFile:Name trong file appsettings.json";

            string sqlQuery = "";
            if (exeConfigName.Equals(MISA_BAMBOO))
                sqlQuery = @"SELECT communeID AS CompanyCode ,CompanyName
                                FROM (SELECT OptionID, OptionValue FROM dbo.DBOption) d
                                PIVOT
                                (
                                    MAX(OptionValue)
                                    FOR OptionID in (communeID,CompanyName)
                                ) PIV";

            if (exeConfigName.Equals(MISA_MIMOSA))
                sqlQuery = @"SELECT CompanyCode,CompanyName
                                FROM (SELECT OptionID, OptionValue FROM dbo.DBOption) d
                                PIVOT
                                (
                                    MAX(OptionValue)
                                    FOR OptionID in (CompanyCode,CompanyName)
                                ) PIV";

            if (string.IsNullOrEmpty(sqlQuery)) return "Query Get đơn vị gửi dữ liệu có giá trị rỗng";

            string msg = Exec.ExecQueryStringOne(sqlQuery, out UnitSendData unit);
            if (msg.Length > 0) _logger.LogError("Xảy ra lỗi khi lấy thông tin đơn vị gửi");
            if (unit == null) _logger.LogError("Thông tin đơn vị gửi rỗng");

            MaDonViQuanHeNganSach = unit.CompanyCode;
            TenDonViQuanHeNganSach = unit.CompanyName;

            return "";
        }

        private string GetReport(out List<TT3442016_B07> outTT3442016_B07)
        {
            string StartDate = "2020-01-01 00:00:00";
            string FromDate = "2020-01-01 00:00:00";
            string ToDate = "2020-12-31 23:59:59";
            string ReceiptPlanActivityTemplateListID = "";
            string ExpensePlanActivityTemplateListID = "";

            string msg = Exec.GetList("Proc_BUR_TT3442016_B07", new { StartDate, FromDate, ToDate, ReceiptPlanActivityTemplateListID, ExpensePlanActivityTemplateListID }, out outTT3442016_B07);
            if (msg.Length > 0) return "Xảy ra lỗi khi Exec store procedure Proc_BUR_TT3442016_B07 lấy dữ liệu BC";
            if (outTT3442016_B07 == null) return "Exec store procedure Proc_BUR_TT3442016_B07 trả về dữ liệu rỗng";

            return "";
        }
    }
}
