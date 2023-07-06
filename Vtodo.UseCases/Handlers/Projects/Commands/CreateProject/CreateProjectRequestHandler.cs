using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using MediatR;
using Vtodo.Entities.Enums;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.UseCases.Handlers.Projects.Commands.CreateProject
{
    internal class CreateProjectRequestHandler : IRequestHandler<CreateProjectRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly ICurrentAccountService _currentAccountService;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public CreateProjectRequestHandler(
            IDbContext dbContext, 
            ICurrentAccountService currentAccountService,
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _currentAccountService = currentAccountService;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task Handle(CreateProjectRequest request, CancellationToken cancellationToken)
        {
            var project = _mapper.Map<Project>(request.CreateProjectDto);
            var account = _currentAccountService.Account;
            
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _projectSecurityService.ChangeOwner(project, account);
        }
    }
}