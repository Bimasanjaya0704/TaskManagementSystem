using AutoMapper;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Web.Api.DTOs;

namespace TaskManagementSystem.Web.Api.Mappings
{
    public class MappingsPresentation : Profile
    {
        public MappingsPresentation()
        {
            CreateMap<ProjectRequestDto, ProjectDTO>();
            CreateMap<ProjectDTO, ProjectRequestDto>();

            CreateMap<ProjectResponseDto, ProjectDTO>();
            CreateMap<ProjectDTO, ProjectResponseDto>();

            CreateMap<TaskRequestDto, TaskDTO>();
            CreateMap<TaskDTO, TaskRequestDto>();

            CreateMap<TaskDTO, TaskResponseDto>();
            CreateMap<TaskResponseDto, TaskDTO>();

            CreateMap<UserRequestDto, UserDTO>();
            CreateMap<UserDTO, UserRequestDto>();
            
            CreateMap<UserDTO, UserResponseDto>(); 
            CreateMap<UserResponseDto, UserDTO>(); 
        }
    }
}
