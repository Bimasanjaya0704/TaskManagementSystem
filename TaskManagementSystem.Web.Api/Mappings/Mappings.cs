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
            CreateMap<ProjectDTO, ProjectResponseDto>();
            
            CreateMap<TaskRequestDto, CreateTaskDto>();
            CreateMap<TaskDTO, TaskResponseDto>();

            CreateMap<UserRequestDto, UserDTO>();
            CreateMap<UserDTO, UserResponseDto>();
            
            CreateMap<AuthenticatedUser, AuthenticatedUserDTO>();
            CreateMap<AuthenticatedUserDTO, AuthenticatedUser>();
        }
    }
}
