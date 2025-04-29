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
            .ForMember(dest => dest.AssignedTasks, opt => opt.MapFrom(src => src.AssignedTasks))  
            .ForMember(dest => dest.ReviewedTasks, opt => opt.MapFrom(src => src.ReviewedTasks));
        CreateMap<UserDTO, UserEntity>();

        CreateMap<ProjectDTO, ProjectEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<ProjectEntity, ProjectDTO>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<RegisterDTO, UserEntity>();
    }
}