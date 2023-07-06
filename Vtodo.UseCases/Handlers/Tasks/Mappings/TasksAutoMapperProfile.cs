using System.Collections.Generic;
using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Mappings
{
    public class TasksAutoMapperProfile : Profile
    {
        public TasksAutoMapperProfile()
        {
            CreateMap<TaskM, TaskDto>()
                .ForMember(x => x.EndDate, opt => opt.Ignore())
                .ForMember(x => x.ParentId, opt => opt.Ignore());
            CreateMap<CreateTaskDto, TaskM>()
                .ForMember(x => x.Board, opt => opt.Ignore())
                .ForMember(x => x.ParentTask, opt => opt.Ignore());
        }
    }
}