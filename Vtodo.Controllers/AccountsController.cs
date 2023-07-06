using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vtodo.UseCases.Handlers.Accounts.Commands.CreateAccount;
using Vtodo.UseCases.Handlers.Accounts.Commands.LoginByPassword;
using Vtodo.UseCases.Handlers.Accounts.Commands.Logout;
using Vtodo.UseCases.Handlers.Accounts.Commands.LogoutAll;
using Vtodo.UseCases.Handlers.Accounts.Commands.RefreshTokens;
using Vtodo.UseCases.Handlers.Accounts.Dto;

namespace Vtodo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Create an account
        /// </summary>
        /// <param name="createAccountDto">Create account Dto</param>
        /// <response code="200">Returns access and refresh tokens</response>
        /// <response code="400">Email is already taken</response>
        /// <response code="400">Username is already taken</response>
        /// <response code="400">Confirm password not equal password</response>
        [AllowAnonymous]
        [HttpPost()]
        public async Task<ActionResult<JwtTokensDto>> Create([FromBody] CreateAccountDto createAccountDto)
        {
            return await _mediator.Send(new CreateAccountRequest() {CreateAccountDto = createAccountDto});
        }
        
        /// <summary>
        /// Generate new access and refresh tokens
        /// </summary>
        /// <param name="refreshTokensDto">Refresh token Dto</param>
        /// <response code="200">Returns access and refresh tokens</response>
        /// <response code="400">Invalid refresh token</response>
        /// <response code="403">Access denied</response>
        [AllowAnonymous]
        [HttpPost("refresh_tokens")]
        public async Task<ActionResult<JwtTokensDto>> RefreshTokens([FromBody] RefreshTokensDto refreshTokensDto)
        {
            return await _mediator.Send(new RefreshTokensRequest() {RefreshTokensDto = refreshTokensDto});
        }
        
        /// <summary>
        /// Login in account
        /// </summary>
        /// <param name="loginByPasswordDto">LoginByPassword Dto (Email + password)</param>
        /// <response code="200">Returns access and refresh tokens</response>
        /// <response code="403">Access denied</response>
        /// <response code="404">Account not found</response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<JwtTokensDto>> Login([FromBody] LoginByPasswordDto loginByPasswordDto)
        {
            return await _mediator.Send(new LoginByPasswordRequest() {LoginByPasswordDto = loginByPasswordDto});
        }
        
        /// <summary>
        /// Logout (Delete current refresh token)
        /// </summary>
        /// <param name="logoutDto">Logout Dto (refresh token)</param>
        /// <response code="200">Deleted</response>
        /// <response code="400">Invalid token</response>
        /// <response code="401">User unauthorized</response>
        /// <response code="403">Access denied</response>
        [HttpPost("Logout")]
        public async Task Logout([FromBody] LogoutDto logoutDto)
        {
            await _mediator.Send(new LogoutRequest() {LogoutDto = logoutDto});
        }
        
        /// <summary>
        /// Logout (Delete all current user refresh tokens)
        /// </summary>
        /// <response code="200">Deleted</response>
        /// <response code="401">User unauthorized</response>
        /// <response code="403">Access denied</response>
        [HttpPost("Logout_all")]
        public async Task LogoutAll()
        {
            await _mediator.Send(new LogoutAllRequest());
        }
    }
}