using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace BT_SendDataMISA
{
    public class GetConnecStringInFile
    {
        public GetConnecStringInFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfiguration _configuration { get; }

        public string GetStringConnectDatabase(string pathConfigFile, out string connectString)
        {
            connectString = "";
            if (string.IsNullOrEmpty(pathConfigFile)) return "Đường dẫn đến file Config không tồn tại";

            string connectStringName = _configuration.GetValue<string>("exeConfigFile:ConnectStringName");
            if (string.IsNullOrEmpty(connectStringName)) return "Không tìm thấy cấu hình exeConfigFile:ConnectStringName trong file appsettings.json";

            string ignoreConnectString = _configuration.GetValue<string>("exeConfigFile:IgnoreConnectString");
            if (string.IsNullOrEmpty(ignoreConnectString)) return "Không tìm thấy cấu hình IgnoreConnectString trong file appsettings.json";

            string passWord = _configuration.GetValue<string>("exeConfigFile:Password");
            if (string.IsNullOrEmpty(passWord)) return "Không tìm thấy cấu hình Password trong file appsettings.json";

            string regexString = _configuration.GetValue<string>("exeConfigFile:RegexString");
            if (string.IsNullOrEmpty(regexString)) return "Không tìm thấy cấu hình RegexString trong file appsettings.json";

            string userId = _configuration.GetValue<string>("exeConfigFile:UserId");
            if (string.IsNullOrEmpty(regexString)) return "Không tìm thấy cấu hình UserId trong file appsettings.json";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathConfigFile.Trim());
                XmlNodeList xnList = doc.SelectNodes("/configuration/connectionStrings");
                if (xnList.Count == 0) return "Không tìm thấy chuỗi kết nối trong file" + pathConfigFile;

                XmlNode xn = xnList[0];
                if (xn.ChildNodes.Count == 0) return "Không tìm thấy chuỗi kết nối trong file" + pathConfigFile;

                foreach (XmlNode node in xn.ChildNodes)
                {
                    var attribute = node.Attributes["name"];
                    if (attribute != null && attribute.Value == connectStringName) connectString = node.Attributes["connectionString"].Value;
                }

                if (connectString.Length == 0) return string.Format(@"Không tìm thấy cấu hình ConnectStringName = {0} trong file appsettings.json", connectStringName);

                Regex regex = new Regex(@"integrated\s+security\=([^\n\r]*)");
                Match match = regex.Match(connectString);

                if (match != null && match.Value.Length > 0) connectString = connectString.Replace(match.Value, "");
                connectString = connectString.Replace(ignoreConnectString, "").Replace("&quot;", "");

                regex = new Regex(regexString);
                match = regex.Match(connectString);
                if (match == null || match.Groups.Count == 0 || match.Length == 0) return "Không lấy được thông tin UserID Login SQL MISA";

                string authentication = string.Format(@"User ID={0};Password={1}", userId, passWord);
                connectString += authentication;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }
    }
}
