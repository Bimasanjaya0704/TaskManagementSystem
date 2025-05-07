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

        CreateMap<ProjectDTO, ProjectEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<ProjectEntity, ProjectDTO>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<RegisterDTO, UserEntity>();
    }
}