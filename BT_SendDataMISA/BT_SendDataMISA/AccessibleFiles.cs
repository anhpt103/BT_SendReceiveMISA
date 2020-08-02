using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BT_SendDataMISA
{
    public class AccessibleFiles
    {
        private readonly ILogger<Worker> _logger;
        private List<string> listIgnore = new List<string>(new string[] { "C:\\inetpub", "C:\\Intel", "C:\\Log", "C:\\MSOCache", "C:\\PerfLogs", "C:\\Windows", "C:\\$Recycle.Bin", "C:\\System.sav", "C:\\ProgramData", "C:\\Temp", "C:\\Users" });
        private string extensionFile = ".exe.Config";

        public AccessibleFiles(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> SearchAccessibleFiles(string root, string searchTerm)
        {
            var files = new List<string>();

            foreach (var file in Directory.EnumerateFiles(root).Where(m => m.Contains(searchTerm)))
            {
                files.Add(file);
            }

            foreach (var subDir in Directory.EnumerateDirectories(root))
            {
                var checkIgnore = listIgnore.IndexOf(subDir);
                if (checkIgnore != -1) continue;

                try
                {
                    files.AddRange(SearchAccessibleFiles(subDir, searchTerm));
                }
                catch { }
            }

            if (files.Count == 0) _logger.LogError("Không tìm thấy file MISA Bamboo.NET.exe.Config trong thư mục: " + root);

            foreach (var file in files)
            {
                if (file.LastIndexOf(extensionFile) != -1)
                {
                    return new string[] { file }; ;
                }
            }

            return files;
        }
    }
}
