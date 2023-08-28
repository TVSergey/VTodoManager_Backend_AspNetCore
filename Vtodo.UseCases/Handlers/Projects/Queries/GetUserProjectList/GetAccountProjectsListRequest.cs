using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Queries.GetUserProjectList
{
    public class GetAccountProjectsListRequest : IRequest<List<ProjectDto>>
    {
        [Required]
        public int UserId { get; set; }
    }
}