using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vtodo.UseCases.Handlers.Tasks.Commands.CreateTask;
using Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTask;
using Vtodo.UseCases.Handlers.Tasks.Commands.DeleteTaskHeaderBackground;
using Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherBoard;
using Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToAnotherTask;
using Vtodo.UseCases.Handlers.Tasks.Commands.MoveTaskToRoot;
using Vtodo.UseCases.Handlers.Tasks.Commands.UpdateTask;
using Vtodo.UseCases.Handlers.Tasks.Commands.UploadTaskHeaderBackground;
using Vtodo.UseCases.Handlers.Tasks.Dto;
using Vtodo.UseCases.Handlers.Tasks.Queries.GetTask;
using Vtodo.UseCases.Handlers.Tasks.Queries.GetTaskHeaderBackground;
using Vtodo.UseCases.Handlers.Tasks.Queries.GetTasksByBoard;

namespace Vtodo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get task by id
        /// </summary>
        /// <param name="id">Task id</param>
        /// <response code="200">Returns task dto</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">task not found</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> Get(int id)
        {
            return await _mediator.Send(new GetTaskRequest() { Id = id});
        }
        
        /// <summary>
        /// Get list of tasks by board
        /// </summary>
        /// <param name="id">Board id</param>
        /// <response code="200">Returns list of task dto</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        /// <response code="404">Task not found</response>
        [HttpGet("by_board/{id}")]
        public async Task<ActionResult<List<TaskDto>>> GetByTask(int id)
        {
            return await _mediator.Send(new GetTasksByBoardRequest() { BoardId = id});
        }
        
        /// <summary>
        /// Create a task
        /// </summary>
        /// <param name="createTaskDto">Task dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Board not found</response>
        /// <response code="404">Task not found</response>
        [HttpPost()]
        public async Task Create([FromBody] CreateTaskDto createTaskDto)
        {
            await _mediator.Send(new CreateTaskRequest() { CreateTaskDto = createTaskDto});
        }
        
        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="updateTaskDto">Task dto</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpPut("{id:int}")]
        public async Task Update(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            await _mediator.Send(new UpdateTaskRequest() { Id = id, UpdateTaskDto = updateTaskDto});
        }
        
        /// <summary>
        /// Move task to root in board
        /// </summary>
        /// <param name="id">Task id</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpPut("{id:int}/moveto/root")]
        public async Task MoveToRoot(int id)
        {
            await _mediator.Send(new MoveTaskToRootRequest() { Id = id});
        }
        
        /// <summary>
        /// Move task to another task as children
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="parentId">Parent task id</param>
        /// <response code="200"></response>
        /// <response code="400">New parent task id should not be equal to task id (Attempt to use self as parent)</response>
        /// <response code="400">New parent task id should not be equal to old parent task id (Attempt to change parent task to same parent)</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpPut("{id:int}/moveto/task/{parentId:int}")]
        public async Task MoveToTask(int id, int parentId)
        {
            await _mediator.Send(new MoveTaskToAnotherTaskRequest() { TaskId = id, NewParentTaskId = parentId});
        }
        
        /// <summary>
        /// Move task to another board
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="boardId">Board id</param>
        /// <response code="200"></response>
        /// <response code="400">New board id should not be equal to old board id (Attempt to change board to same board)</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        /// <response code="404">Board not found</response>
        [HttpPut("{id:int}/moveto/board/{boardId:int}")]
        public async Task MoveToBoard(int id, int boardId)
        {
            await _mediator.Send(new MoveTaskToAnotherBoardRequest() { TaskId = id, NewBoardId = boardId});
        }
        
        
        /// <summary>
        /// Get task header background image
        /// </summary>
        /// <param name="id">Task id</param>
        /// <response code="200"></response>
        /// <response code="400">Background image file not exists</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        /// <response code="500">File not found</response>
        [HttpGet("{id:int}/header_background")]
        public async Task<ActionResult<FileStream>> GetTaskHeaderBackground(int id)
        {
            var file = await _mediator.Send(new GetTaskHeaderBackgroundRequest() {Id = id});
            
            return File(file, "application/octet-stream", Path.GetFileName(file.Name));
        }
        
        /// <summary>
        /// Upload task header background image
        /// </summary>
        /// <param name="id">Task id</param>
        /// <param name="uploadTaskHeaderBackground">Image, one of the types: ".jpg", ".png"</param>
        /// <response code="200"></response>
        /// <response code="400">Invalid file</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpPut("{id:int}/header_background")]
        public async Task UploadTaskHeaderBackground(int id, IFormFile uploadTaskHeaderBackground)
        {
            await _mediator.Send(new UploadTaskHeaderBackgroundRequest () { Id = id, BackgroundImage = uploadTaskHeaderBackground.OpenReadStream(), FileName = uploadTaskHeaderBackground.FileName });
        }
        
        /// <summary>
        /// Delete task header background image
        /// </summary>
        /// <param name="id">Task id</param>
        /// <response code="200"></response>
        /// <response code="400">Background image file not exists</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        /// <response code="500">File not found</response>
        [HttpPut("{id:int}/delete_header_background")]
        public async Task DeleteTaskHeaderBackground(int id)
        {
            await _mediator.Send(new DeleteTaskHeaderBackgroundRequest () { Id = id});
        }

        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="id">Task id</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Task not found</response>
        [HttpDelete("{id:int}")]
        public async Task Delete(int id)
        {
            await _mediator.Send(new DeleteTaskRequest() { Id = id});
        }
        
    }
}