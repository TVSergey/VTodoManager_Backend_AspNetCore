using MediatR;
using Vtodo.UseCases.Handlers.Tasks.Dto;

namespace Vtodo.UseCases.Handlers.Tasks.Queries.GetTask
{
    public class GetTaskRequest : IRequest<TaskDto>
    {
        public int Id { get; set; }
    }
}