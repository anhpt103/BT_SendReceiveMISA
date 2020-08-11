using AutoMapper;
using BT_SendDataMISA.Models;
using BT_SendDataMISA.Models.Report;

namespace BT_SendDataMISA
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DbMisaInfo, ReportHeader>();
        }
    }
}
