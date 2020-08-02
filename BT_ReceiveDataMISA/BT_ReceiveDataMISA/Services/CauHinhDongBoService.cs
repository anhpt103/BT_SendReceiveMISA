using BT_ReceiveDataMISA.Constanst;
using BT_ReceiveDataMISA.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BT_ReceiveDataMISA.Services
{
    public interface ICauHinhDongBoService
    {
        string GetSetting(string clientKey, out CauHinhDongBo outSetting);
    }

    public class CauHinhDongBoService : ICauHinhDongBoService
    {
        private readonly ReceiveDataMISAContext _dbContext;
        private readonly ILogger<CauHinhDongBoService> _logger;

        public CauHinhDongBoService(ReceiveDataMISAContext dbContext, ILogger<CauHinhDongBoService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public string GetSetting(string clientKey, out CauHinhDongBo outSetting)
        {
            outSetting = null;

            try
            {
                outSetting = _dbContext.Settings.FirstOrDefault(s => s.ClientKey == clientKey);
                if (outSetting == null) return Msg.SETTING_NOTFOUND;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return "";
        }
    }
}
