using BT_ReceiveDataMISA.Constanst;
using BT_ReceiveDataMISA.Entities;
using BT_ReceiveDataMISA.Models;
using BT_ReceiveDataMISA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BT_ReceiveDataMISA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CauHinhDongBoController : ControllerBase
    {
        private readonly ILogger<CauHinhDongBoController> _logger;
        private ICauHinhDongBoService _cauHinhDongBoService;

        public CauHinhDongBoController(ILogger<CauHinhDongBoController> logger, ICauHinhDongBoService cauHinhDongBoService)
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

        [HttpPost("Post_TT3442016_B07")]
        public IActionResult Post_TT3442016_B07(List<TT3442016_B07> listTT3442016_B07)
        {
            return Ok(listTT3442016_B07);
        }
    }
}
