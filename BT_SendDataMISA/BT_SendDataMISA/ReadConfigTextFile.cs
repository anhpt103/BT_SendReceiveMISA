using Microsoft.Extensions.Configuration;
using System.IO;

namespace BT_SendDataMISA
{
    public class ReadConfigTextFile
    {
        public ReadConfigTextFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfiguration _configuration { get; }

        public string ReadFile(out string outStr)
        {
            outStr = "";
            string pathFile = _configuration.GetValue<string>("AppSettings:PathConfigTextFile");
            string fileName = _configuration.GetValue<string>("AppSettings:FileTextName");

            if (string.IsNullOrEmpty(pathFile)) return "Không tìm thấy cấu hình đường dẫn trong file appsettings.json";
            if (string.IsNullOrEmpty(fileName)) return "Không tìm thấy cấu hình tên file trong file appsettings.json";

            if (!File.Exists(pathFile + fileName)) return "File không tồn tại";

            outStr = File.ReadAllText(pathFile + fileName);
            return "";
        }
    }
}
