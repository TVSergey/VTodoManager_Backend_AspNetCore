using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vtodo.UseCases.Handlers.Boards.Commands.CreateBoard;
using Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoard;
using Vtodo.UseCases.Handlers.Boards.Commands.DeleteBoardHeaderBackground;
using Vtodo.UseCases.Handlers.Boards.Commands.MoveBoardToAnotherProject;
using Vtodo.UseCases.Handlers.Boards.Commands.UpdateBoard;
using Vtodo.UseCases.Handlers.Boards.Commands.UploadBoardHeaderBackground;
using Vtodo.UseCases.Handlers.Boards.Dto;
using Vtodo.UseCases.Handlers.Boards.Queries.GetBoard;
using Vtodo.UseCases.Handlers.Boards.Queries.GetBoardHeaderBackground;
using Vtodo.UseCases.Handlers.Boards.Queries.GetBoardsByProject;

namespace Vtodo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public BoardsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get board by id
        /// </summary>
        /// <param name="id">Board id</param>
        /// <response code="200">Returns board dto</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BoardDto>> Get(int id)
        {
            return await _mediator.Send(new GetBoardRequest() { Id = id});
        }
        
        /// <summary>
        /// Get all boards by project id
        /// </summary>
        /// <param name="id">project id</param>
        /// <response code="200">Returns list board dto</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpGet("by_project/{id}")]
        public async Task<ActionResult<List<BoardDto>>> GetByTask(int id)
        {
            return await _mediator.Send(new GetBoardsByProjectRequest() { ProjectId = id});
        }
        
        /// <summary>
        /// Create a board
        /// </summary>
        /// <param name="createBoardDto">Create board Dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        [HttpPost()]
        public async Task Create([FromBody] CreateBoardDto createBoardDto)
        {
            await _mediator.Send(new CreateBoardRequest() { CreateBoardDto = createBoardDto});
        }
        
        /// <summary>
        /// Update board
        /// </summary>
        /// <param name="id">Board id</param>
        /// <param name="updateBoardDto">Update board Dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        [HttpPut("{id:int}")]
        public async Task Update(int id, [FromBody] UpdateBoardDto updateBoardDto)
        {
            await _mediator.Send(new UpdateBoardRequest() { Id = id, UpdateBoardDto = updateBoardDto});
        }
        
        /// <summary>
        /// Move board to another project
        /// </summary>
        /// <param name="boardId">Board id</param>
        /// <param name="projectId">New project id</param>
        /// <response code="200"></response>
        /// <response code="400">New project id should not be equal to old project id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        /// <response code="404">Project not found</response>
        [HttpPut("{boardId:int}/moveto/project/{projectId:int}")]
        public async Task MoveToProject(int boardId, int projectId)
        {
            await _mediator.Send(new MoveBoardToAnotherProjectRequest() { BoardId = boardId, ProjectId = projectId});
        }
        
        /// <summary>
        /// Get board header background image 
        /// </summary>
        /// <param name="id">Board id</param>
        /// <response code="200"></response>
        /// <response code="400">Background image file not exists</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        /// <response code="500">File not found</response>
        [HttpGet("{id:int}/header_background")]
        public async Task<ActionResult<FileStream>> GetBoardHeaderBackground(int id)
        {
            var file = await _mediator.Send(new GetBoardHeaderBackgroundRequest() {Id = id});
            
            return File(file, "application/octet-stream", Path.GetFileName(file.Name));
        }
        
        /// <summary>
        /// Upload board header background image 
        /// </summary>
        /// <param name="id">Board id</param>
        /// <param name="uploadBoardHeaderBackground">Image, one of the types: ".jpg", ".png"</param>
        /// <response code="200"></response>
        /// <response code="400">Background image file not exists</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        [HttpPut("{id:int}/header_background")]
        public async Task UploadBoardHeaderBackground(int id, IFormFile uploadBoardHeaderBackground)
        {
            await _mediator.Send(new UploadBoardHeaderBackgroundRequest () { Id = id, BackgroundImage = uploadBoardHeaderBackground.OpenReadStream(), FileName = uploadBoardHeaderBackground.FileName });
        }
        
        /// <summary>
        /// Delete board header background image 
        /// </summary>
        /// <param name="id">Board id</param>
        /// <response code="200"></response>
        /// <response code="400">Background image file not exists</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        [HttpPut("{id:int}/delete_header_background")]
        public async Task DeleteBoardHeaderBackground(int id)
        {
            await _mediator.Send(new DeleteBoardHeaderBackgroundRequest () { Id = id});
        }
        
        /// <summary>
        /// Delete board
        /// </summary>
        /// <param name="id">Board id</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            await _mediator.Send(new DeleteBoardRequest() { Id = id});
        }
    }
}