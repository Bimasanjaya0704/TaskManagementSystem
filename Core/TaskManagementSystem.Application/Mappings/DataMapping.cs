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
        CreateMap<UpdateUserDto, UserEntity>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            /*.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))*/
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectMemberships, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.FriendshipsRequested, opt => opt.Ignore())
            .ForMember(dest => dest.FriendshipsReceived, opt => opt.Ignore());


        CreateMap<ProjectDTO, ProjectEntity>();
        CreateMap<ProjectEntity, ProjectDTO>();
        CreateMap<RegisterDTO, UserEntity>();

        CreateMap<FriendshipDto, FriendRequestEntity>();
        CreateMap<FriendRequestEntity, FriendshipDto>();
    }
}