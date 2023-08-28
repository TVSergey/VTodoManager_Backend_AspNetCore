using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vtodo.Entities.Exceptions;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.UseCases.Handlers.Projects.Dto;

namespace Vtodo.UseCases.Handlers.Projects.Queries.GetUserProjectList
{
    internal class GetAccountProjectsListRequestHandler : IRequestHandler<GetAccountProjectsListRequest, List<ProjectDto>>
    {
        private readonly IDbContext _dbContext;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IMapper _mapper;
        
        public GetAccountProjectsListRequestHandler(
            IDbContext dbContext, 
            IProjectSecurityService projectSecurityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _projectSecurityService = projectSecurityService;
            _mapper = mapper;
        }
        
        public async Task<List<ProjectDto>> Handle(GetAccountProjectsListRequest request, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);
            if (account == null) throw new AccountNotFoundException();

            var projects = await _dbContext.ProjectAccountsRoles
                .AsNoTracking()
                .Where(x => x.Account.Id == account.Id)
                .Select(x => x.Project)
                .ToListAsync(cancellationToken);

            if (projects?.Count == 0 || projects == null) throw new ProjectNotFoundException();

            var result = _mapper.Map<List<ProjectDto>>(projects);

            for (var i = 0; i < result.Count; i++)
            {
                var project = projects.FirstOrDefault(x=> x.Id == result.ElementAt(i).Id);
                if (project == null) continue;

                result[i].CreationDate = new DateTimeOffset(project.CreationDate).ToUnixTimeMilliseconds();
            }

            return result;
        }
    }
}