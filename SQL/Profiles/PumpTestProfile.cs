using AutoMapper;
using SQL.Entity;

namespace SQL.Profiles
{
    ////public class PumpTestDtoProfile : Profile
    ////{
    ////    public PumpTestDtoProfile()
    ////    {
    ////        CreateMap<PumpTest, PumpTestDto>().ReverseMap();
    ////    }
    ////}

    public class PumpTestProfile : Profile
    {
        public PumpTestProfile()
        {
            CreateMap<PumpTestDto, PumpTest>().ReverseMap();
        }
    }
}
