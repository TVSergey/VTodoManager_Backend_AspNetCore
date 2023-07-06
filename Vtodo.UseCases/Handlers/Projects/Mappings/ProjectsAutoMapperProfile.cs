using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Mappings
{
    public class ProjectsAutoMapperProfile : Profile
    {
        public ProjectsAutoMapperProfile()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(x => x.CreationDate, opt => opt.Ignore());
            CreateMap<CreateProjectDto, Project>();
        }
    }
}