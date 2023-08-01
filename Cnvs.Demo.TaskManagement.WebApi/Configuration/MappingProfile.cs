using Cnvs.Demo.TaskManagement.Domain;
using Task = Cnvs.Demo.TaskManagement.Domain.Task;

namespace Cnvs.Demo.TaskManagement.WebApi.Configuration;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<Cnvs.Demo.TaskManagement.Dto.User, User>();
        CreateMap<User, Cnvs.Demo.TaskManagement.Dto.User>();

        CreateMap<Cnvs.Demo.TaskManagement.Dto.Task, Task>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (TaskState)src.State))
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser))
            .ForMember(dest => dest.AssignedUsersHistory, opt => opt.MapFrom(src => src.AssignedUsersHistory));

        
        CreateMap<NullUser, Cnvs.Demo.TaskManagement.Dto.User>().ConstructUsing(src => (null as Cnvs.Demo.TaskManagement.Dto.User)!);

        CreateMap<Task, Cnvs.Demo.TaskManagement.Dto.Task>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (Cnvs.Demo.TaskManagement.Dto.TaskState)src.State))
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser))
            .ForMember(dest => dest.AssignedUsersHistory, opt => opt.MapFrom(src => src.AssignedUsersHistory))
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser is NullUser ? null : src.AssignedUser));
    }
}