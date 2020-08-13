using System.Collections.Generic;
using static BT_SendDataMISA.Models.AccessToken;

namespace BT_SendDataMISA.Models
{
    public class ParamSetting
    {
        public TokenInfo TokenInfos { get; set; }
        public List<SysScheduler> ListSysScheduler { get; set; }
    }
}
