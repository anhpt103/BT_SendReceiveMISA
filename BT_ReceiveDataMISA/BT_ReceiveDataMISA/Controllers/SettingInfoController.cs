using BT_ReceiveDataMISA.Constanst;
using BT_ReceiveDataMISA.Entities;
using BT_ReceiveDataMISA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BT_ReceiveDataMISA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingInfoController : ControllerBase
    {
        private readonly ILogger<SettingInfoController> _logger;
        private ICauHinhDongBoService _cauHinhDongBoService;

        public SettingInfoController(ILogger<SettingInfoController> logger, ICauHinhDongBoService cauHinhDongBoService)
        {
            _logger = logger;
            _cauHinhDongBoService = cauHinhDongBoService;
        }

        [HttpGet("GetSetting/{clientKey}")]
        public IActionResult GetSetting(string clientKey)
        {
            if (string.IsNullOrEmpty(clientKey)) { _logger.LogError(Msg.CLIENTKEY_ISNULL_EMPTY); return NotFound(); }

            string msg = _cauHinhDongBoService.GetSetting(clientKey, out CauHinhDongBo outSetting);
            if (msg.Length > 0) { _logger.LogError(msg); return NotFound(); }

            return Ok(outSetting);
        }

    }
}
