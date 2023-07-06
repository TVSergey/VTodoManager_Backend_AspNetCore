using Microsoft.Extensions.Options;
using Moq;
using Vtodo.Infrastructure.Implementation.Options;
using Vtodo.Infrastructure.Implementation.Services;
using Xunit;

namespace Vtodo.Infrastructure.Implementation.Tests.Unit
{
    public class ConfigServiceTest
    {
        [Fact]
        public void HasherIterations_GetHasherIterations_ReturnsInt()
        {
            var expectedValue = SetupHasherOptions().Object.Value.Iterations;
            var actualValue = SetupConfigService().HasherIterations;
            
            Assert.True(actualValue > 0);
            Assert.Equal(expectedValue, actualValue);
        }
        
        [Fact]
        public void HasherKeySize_GetHasherKeySize_ReturnsInt()
        {
            var expectedValue = SetupHasherOptions().Object.Value.KeySize;
            var actualValue = SetupConfigService().HasherKeySize;
            
            Assert.True(actualValue > 0);
            Assert.Equal(expectedValue, actualValue);
        }
        
        [Fact]
        public void MaxProjectFileSizeInMb_GetMaxProjectFileSizeInMb_ReturnsInt()
        {
            var expectedValue = SetupProjectFilesOptions().Object.Value.MaxProjectFileSizeInMb;
            var actualValue = SetupConfigService().MaxProjectFileSizeInMb;
            
            Assert.True(actualValue > 0);
            Assert.True(actualValue <= 1024);
            Assert.Equal(expectedValue, actualValue);
        }
        
        private static Mock<IOptions<HasherOptions>> SetupHasherOptions()
        {
            var hasherOptions = new Mock<IOptions<HasherOptions>>();
            hasherOptions.Setup(x => x.Value).Returns(new HasherOptions()
            {
                KeySize = 64,
                Iterations = 400000
            });

            return hasherOptions;
        }
        
        private static Mock<IOptions<ProjectFilesOptions>> SetupProjectFilesOptions()
        {
            var projectFilesOptions = new Mock<IOptions<ProjectFilesOptions>>();
            projectFilesOptions.Setup(x => x.Value).Returns(new ProjectFilesOptions()
            {
                MaxProjectFileSizeInMb = 512
            });

            return projectFilesOptions;
        }

        private static ConfigService  SetupConfigService()
        {
            return new ConfigService(SetupHasherOptions().Object, SetupProjectFilesOptions().Object);
        }
    }
}