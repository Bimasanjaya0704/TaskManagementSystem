using AutoMapper;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mappings;

public class DataMapping : Profile
{
    public DataMapping()
    {
        CreateMap<TaskEntity, TaskDTO>();
        CreateMap<TaskDTO, TaskEntity>();

        CreateMap<UserEntity, UserDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        CreateMap<UserDTO, UserEntity>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<ProjectDTO, ProjectEntity>();
        CreateMap<ProjectEntity, ProjectDTO>();
        CreateMap<RegisterDTO, UserEntity>();
        
        CreateMap<FriendshipDto, FriendRequestEntity>();
        CreateMap<FriendRequestEntity, FriendshipDto>();
    }
}