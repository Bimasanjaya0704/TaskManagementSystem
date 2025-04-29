using AutoMapper;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mappings;

public class TaskMapping : Profile
{
    public TaskMapping()
    {
        CreateMap<TaskEntity, TaskDTO>();
        CreateMap<TaskDTO, TaskEntity>();
        
        CreateMap<UserEntity, UserDTO>();
        CreateMap<UserDTO, UserEntity>();
    }
}