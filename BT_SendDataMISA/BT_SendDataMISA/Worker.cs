using BT_SendDataMISA.Common;
using BT_SendDataMISA.Function;
using BT_SendDataMISA.HttpClientAPI;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Report;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BT_SendDataMISA.Models.AccessToken;

namespace BT_SendDataMISA
{
    public class Worker : BackgroundService
    {
        private string TenUngDung = "";
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
            TenUngDung = _configuration.GetValue<string>("exeConfigFile:TenUngDung");
            if (string.IsNullOrEmpty(TenUngDung)) { _logger.LogError("Không tìm thấy cấu hình exeConfigFile:TenUngDung trong file appsettings.json"); return; }

            string urlAPI = _configuration.GetValue<string>("WebServer:UrlAPI");
            if (string.IsNullOrEmpty(urlAPI)) { _logger.LogError("Không tìm thấy cấu hình WebServer:UrlAPI trong file appsettings.json"); return; }

            Result result = await GetAccessTokenAsync(urlAPI);
            if (result.IsFailed)
            {
                IEnumerable<Reason> reasons = result.Reasons;
                _logger.LogError(reasons.FirstOrDefault().Message);
                return;
            }

            IEnumerable<Success> successes = result.Successes;
            Convertor.StringToObject(successes.FirstOrDefault().Message, out TokenInfo accessToken);
            string msg = DoBeginProcessSync();
            if (msg.Length > 0) _logger.LogError(msg);

            // Get SysScheduler From Root Database
            result = await GetSysScheduler(urlAPI, accessToken.access_token);
            //

            // Get StartDatabase Misa
            msg = CommonFunction.GetStartDateMisa(out string startDate);
            if (msg.Length > 0) _logger.LogError(msg);
            //

            B02BCTC_Sync b02BCTC_Sync = new B02BCTC_Sync();
            b02BCTC_Sync.GetDataReport(startDate);

            while (!stoppingToken.IsCancellationRequested)
            {

                // HttpClientPost httpClientPost = new HttpClientPost();
                // Result response = await httpClientPost.SendsRequest(urlAPI + "CauHinhDongBo/Post_TT3442016_B07", outTT3442016_B07);

                await Task.Delay((60 * 1000 * 60), stoppingToken);
            }
        }

        private async Task<Result> GetAccessTokenAsync(string urlAPI)
        {
            string api = _configuration.GetValue<string>("AuthenToken:api");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình AuthenToken:api trong file appsettings.json");

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
            return await httpClientPost.SendsRequest("http://api.btsoftvn.com:8383/auth/token"/*urlAPI + api*/, tokenParam);
        }

        private async Task<Result> GetSysScheduler(string urlAPI, string token)
        {
            string api = _configuration.GetValue<string>("ApiName:GetScheduler");
            if (string.IsNullOrEmpty(api)) return Result.Fail("Không tìm thấy cấu hình ApiName:GetScheduler trong file appsettings.json");

            HttpClientPost httpClientPost = new HttpClientPost();
            return await httpClientPost.SendsRequestWithToken(urlAPI + api, token);
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
