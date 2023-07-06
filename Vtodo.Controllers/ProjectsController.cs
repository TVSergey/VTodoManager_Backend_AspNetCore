using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vtodo.UseCases.Handlers.Projects.Commands.CreateProject;
using Vtodo.UseCases.Handlers.Projects.Commands.DeleteProject;
using Vtodo.UseCases.Handlers.Projects.Commands.UpdateProject;
using Vtodo.UseCases.Handlers.Projects.Dto;
using Vtodo.UseCases.Handlers.Projects.Queries.GetProject;

namespace Vtodo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get project by id
        /// </summary>
        /// <param name="id">Project id</param>
        /// <response code="200">Returns project dto</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> Get(int id)
        {
           return await _mediator.Send(new GetProjectRequest() { Id = id});
        }
        
        /// <summary>
        /// Create a project
        /// </summary>
        /// <param name="createProjectDto">Create project dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        [HttpPost()]
        public async Task Create([FromBody] CreateProjectDto createProjectDto)
        {
            await _mediator.Send(new CreateProjectRequest() { CreateProjectDto = createProjectDto});
        }
        
        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="updateProjectDto">Update board Dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpPut("{id:int}")]
        public async Task Update(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            await _mediator.Send(new UpdateProjectRequest() { Id = id, UpdateProjectDto = updateProjectDto});
        }
        
        /// <summary>
        /// Delete project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            await _mediator.Send(new DeleteProjectRequest { Id = id});
        }
    }
}