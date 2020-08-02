using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace BT_SendDataMISA
{
    public class WriteConfigTextFile
    {
        public WriteConfigTextFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfiguration _configuration { get; }

        public string WriteToFile(string pathConfig, out string outStr)
        {
            outStr = "";
            string pathFile = _configuration.GetValue<string>("AppSettings:PathConfigTextFile");
            string fileName = _configuration.GetValue<string>("AppSettings:FileTextName");

            if (string.IsNullOrEmpty(pathFile)) return "Không tìm thấy cấu hình đường dẫn trong file appsettings.json";
            if (string.IsNullOrEmpty(fileName)) return "Không tìm thấy cấu hình tên file trong file appsettings.json";

            try
            {
                if (!Directory.Exists(pathFile)) Directory.CreateDirectory(pathFile);

                using (FileStream fs = File.Create(pathFile + fileName))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(pathConfig);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            outStr = pathConfig;
            return "";
        }

    }
}
