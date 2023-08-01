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
            .ForMember(dest => dest.AssignedUser, opt => opt.NullSubstitute(NullUser.Instance))
            .ForMember(dest => dest.AssignedUsersHistory, opt => opt.MapFrom(src => src.AssignedUsersHistory));

        CreateMap<Task, Cnvs.Demo.TaskManagement.Dto.Task>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (Cnvs.Demo.TaskManagement.Dto.TaskState)src.State))
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser == NullUser.Instance ? null : src.AssignedUser))
            .AfterMap((src, dest) =>
            {
                dest.AssignedUsersHistory = dest.AssignedUsersHistory
                    .Where(user => user.GetType() != typeof(NullUser))
                    .ToArray();
            });
    }
}