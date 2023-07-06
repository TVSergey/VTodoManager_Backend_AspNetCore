using System;
using Microsoft.Extensions.Options;
using Vtodo.Infrastructure.Implementation.Options;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class ConfigService : IConfigService
    {
        private readonly HasherOptions _hasherOptions;
        private readonly ProjectFilesOptions _projectFilesOptions;
        
        public ConfigService(IOptions<HasherOptions> hasherOptions, IOptions<ProjectFilesOptions> projectFilesOptions)
        {
            _hasherOptions = hasherOptions.Value;
            _projectFilesOptions = projectFilesOptions.Value;
        }
        
        public int HasherIterations => _hasherOptions.Iterations;

        public int HasherKeySize => _hasherOptions.KeySize;

        public int MaxProjectFileSizeInMb => _projectFilesOptions.MaxProjectFileSizeInMb;
    }
}