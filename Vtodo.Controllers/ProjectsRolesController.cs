using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.AddMember;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.ChangeOwner;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.GrantRole;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeAllRole;
using Vtodo.UseCases.Handlers.ProjectsRoles.Commands.RevokeRole;
using Vtodo.UseCases.Handlers.ProjectsRoles.Dto;

namespace Vtodo.Controllers
{
    [Route("api/v1/project/{id:int}/roles/")]
    [ApiController]
    public class ProjectsRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Grant role (One of ProjectAdmin, ProjectUpdate)
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="role">Grant role Dto</param>
        /// <response code="200"></response>
        /// <response code="400">Account not a member in project</response>
        /// <response code="400">Account already has this role</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Attempt change owner or add member</response>
        [HttpPost("grant/")]
        public async Task Grant(int id, [FromBody] GrantRoleDto role)
        {
            await _mediator.Send(new GrantRoleRequest() { ProjectId = id, GrantRoleDto = role});
        }
        
        /// <summary>
        /// Revoke role (One of ProjectAdmin, ProjectUpdate, ProjectMember)
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="role">Revoke role Dto</param>
        /// <response code="200"></response>
        /// <response code="400">Account is not a member in project</response>
        /// <response code="400">Account already has this role</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        /// <response code="404">Account with this role not found in project</response>
        /// <response code="500">Attempt remove owner</response>
        [HttpDelete("revoke/")]
        public async Task Revoke(int id, [FromBody] RevokeRoleDto role)
        {
            await _mediator.Send(new RevokeRoleRequest() { ProjectId = id, RevokeRoleDto = role});
        }
        
        /// <summary>
        /// Revoke all roles (Kick member) 
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="role">Kick member Dto</param>
        /// <response code="200"></response>
        /// <response code="400">Account already has this role</response>
        /// <response code="400">Account is not a member in project</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        /// <response code="500">Attempt remove owner</response>
        [HttpDelete("kick_member/")]
        public async Task RevokeAll(int id, [FromBody] KickMemberDto role)
        {
            await _mediator.Send(new KickMemberRequest() { ProjectId = id, KickMemberDto = role});
        }
        
        /// <summary>
        /// Grant member role to account
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="addMemberDto">Add member Dto</param>
        /// <response code="200"></response>
        /// <response code="400">Account already has this role</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        /// <response code="404">Account not found</response>
        [HttpPost("add_member/")]
        public async Task AddMember(int id, [FromBody] AddMemberDto addMemberDto)
        {
            await _mediator.Send(new AddMemberRequest() { ProjectId = id, AddMemberDto = addMemberDto});
        }

        /// <summary>
        /// Change project owner
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="changeOwnerDto">Change owner Dto</param>
        /// <response code="200"></response>
        /// <response code="400">Account is not a member in project</response>
        /// <response code="400">Account already has this role</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Project not found</response>
        /// <response code="404">Account not found</response>
        [HttpPut("change_owner/")]
        public async Task ChangeOwner(int id, [FromBody] ChangeOwnerDto changeOwnerDto)
        {
            await _mediator.Send(new ChangeOwnerRequest() { ProjectId = id, ChangeOwnerDto = changeOwnerDto});
        }
    }
}