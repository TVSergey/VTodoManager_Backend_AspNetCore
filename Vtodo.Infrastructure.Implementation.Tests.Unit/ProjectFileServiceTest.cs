using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vtodo.DataAccess.Postgres;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Implementation.Services;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.Tests.Utils;
using Xunit;

namespace Vtodo.Infrastructure.Implementation.Tests.Unit
{
    public class ProjectFileServiceTest
    {
        private AppDbContext _dbContext = null!;

        [Fact]
        public void CheckFile_SuccessfulCheckFile_ReturnsStringExtension()
        {
            SetupDbContext();
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext,
                SetupMockConfigService().Object,
                SetupMockProjectSecurityService().Object,
                SetupMockFileManagerService().Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes("test");
                memStream.Write(testString, 0, testString.Length );

                var extensionArray = new[] {".png", ".jpg"};
                
                var result = projectsFilesService.CheckFile(memStream, "test.png", extensionArray);
                
                Assert.Contains(result, extensionArray);
            }

            CleanUp();
        }
        
        [Fact]
        public void CheckFile_InvalidFileZeroLength_ThrowsInvalidFileException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext,
                SetupMockConfigService().Object,
                SetupMockProjectSecurityService().Object,
                SetupMockFileManagerService().Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var extensionArray = new[] {".png", ".jpg"};
                
                Assert.Throws<InvalidFileException>(() => projectsFilesService.CheckFile(memStream, "test.png", extensionArray));
            }

            CleanUp();
        }
        
        [Fact]
        public void CheckFile_InvalidFileLengthIsLargeThanMaxSize_ThrowsInvalidFileException()
        {
            SetupDbContext();

            var configMock = SetupMockConfigService();
            configMock.Setup(x => x.MaxProjectFileSizeInMb).Returns(1);
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext,
                configMock.Object,
                SetupMockProjectSecurityService().Object,
                SetupMockFileManagerService().Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500000000));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                Assert.Throws<InvalidFileException>(() => projectsFilesService.CheckFile(memStream, "test.png", extensionArray));
            }

            CleanUp();
        }
        
        [Fact]
        public void CheckFile_InvalidFileEmptyExtension_ThrowsInvalidFileException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext,
                SetupMockConfigService().Object,
                SetupMockProjectSecurityService().Object,
                SetupMockFileManagerService().Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                Assert.Throws<InvalidFileException>(() => projectsFilesService.CheckFile(memStream, "test", extensionArray));
            }

            CleanUp();
        }
        
        [Fact]
        public void CheckFile_InvalidFileInvalidExtension_ThrowsInvalidFileException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext,
                SetupMockConfigService().Object,
                SetupMockProjectSecurityService().Object,
                SetupMockFileManagerService().Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                Assert.Throws<InvalidFileException>(() => projectsFilesService.CheckFile(memStream, "test.test", extensionArray));
            }

            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Project_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object,
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(_dbContext.Projects.First(x => x.Id == 1), "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Board_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1), 
                    _dbContext.Boards.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Task_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1), 
                    _dbContext.Tasks.First(x => x.Id == 1), 
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Project_FileNotFoundInFileSystem_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService.Setup(x => x.OpenFile(It.IsAny<string>())).Returns((FileStream)null!);
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Board_FileNotFoundInFileSystem_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService.Setup(x => x.OpenFile(It.IsAny<string>())).Returns((FileStream)null!);
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1), 
                    _dbContext.Boards.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void GetProjectFile_Task_FileNotFoundInFileSystem_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService.Setup(x => x.OpenFile(It.IsAny<string>())).Returns((FileStream)null!);
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.GetProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1), 
                    _dbContext.Tasks.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void UploadProjectFile_Project_SuccessfulAddFile_ReturnsStringFileName()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.SaveFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult("test.png"));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                projectsFilesService.UploadProjectFile(_dbContext.Projects.First(x => x.Id == 1), memStream, "test");
            }
            
            Assert.NotNull(_dbContext.ProjectFiles
                .FirstOrDefault(x => x.ProjectId == _dbContext.Projects.First(d => d.Id == 1).Id && 
                                    x.FileName == "test.png"));
            
            CleanUp();
        }

        [Fact]
        public void UploadProjectFile_Board_SuccessfulAddFile_ReturnsStringFileName()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.SaveFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult("test.png"));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                projectsFilesService.UploadProjectFile(_dbContext.Projects.First(x => x.Id == 1), _dbContext.Boards.First(d => d.Id == 1), memStream, "test");
            }
            
            Assert.NotNull(_dbContext.ProjectBoardsFiles
                .FirstOrDefault(x => x.ProjectId == _dbContext.Projects.First(d => d.Id == 1).Id && 
                                     x.BoardId == _dbContext.Boards.First(d => d.Id == 1).Id &&
                                     x.FileName == "test.png"));
            
            CleanUp();
        }
        
        [Fact]
        public void UploadProjectFile_Task_SuccessfulAddFile_ReturnsStringFileName()
        {
            SetupDbContext();

            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.SaveFile(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult("test.png"));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);

            using (MemoryStream memStream = new MemoryStream())
            {
                var testString = new UnicodeEncoding().GetBytes(new string('*', 500));
                memStream.Write(testString, 0, testString.Length );
                
                var extensionArray = new[] {".png", ".jpg"};
                
                projectsFilesService.UploadProjectFile(_dbContext.Projects.First(x => x.Id == 1), _dbContext.Tasks.First(d => d.Id == 1), memStream, "test");
            }
            
            Assert.NotNull(_dbContext.ProjectTasksFiles
                .FirstOrDefault(x => x.ProjectId == _dbContext.Projects.First(d => d.Id == 1).Id && 
                                     x.TaskId == _dbContext.Tasks.First(d => d.Id == 1).Id &&
                                     x.FileName == "test.png"));
            
            CleanUp();
        }

        [Fact]
        public void DeleteProjectFile_Project_SuccessfulDeleteProjectFile_ReturnsVoid()
        {
            SetupDbContext();
            _dbContext.ProjectFiles.AddRange(GetProjectFilesList());
            _dbContext.SaveChanges();
            
            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.DeleteFile(It.IsAny<string>()));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            projectsFilesService.DeleteProjectFile(_dbContext.Projects.First(x => x.Id == 1), "test.png");
            
            Assert.Null(_dbContext.ProjectFiles.FirstOrDefault(x => x.ProjectId == 1 && x.FileName == "test.png"));
            
            CleanUp();
        }

        [Fact]
        public void DeleteProjectFile_Board_SuccessfulDeleteProjectBoardFile_ReturnsVoid()
        {
            SetupDbContext();
            _dbContext.ProjectBoardsFiles.AddRange(GetProjectBoardFilesList());
            _dbContext.SaveChanges();
            
            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.DeleteFile(It.IsAny<string>()));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            projectsFilesService.DeleteProjectFile(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Boards.First(x => x.Id == 1),
                "test.png");
            
            Assert.Null(_dbContext.ProjectBoardsFiles.FirstOrDefault(x => x.ProjectId == 1 && x.BoardId == 1 && x.FileName == "test.png"));
            
            CleanUp();
        }
        
        [Fact]
        public void DeleteProjectFile_Task_SuccessfulDeleteProjectTaskFile_ReturnsVoid()
        {
            SetupDbContext();
            _dbContext.ProjectTasksFiles.AddRange(GetProjectTaskFilesList());
            _dbContext.SaveChanges();
            
            var mockFileManagerService = SetupMockFileManagerService();
            mockFileManagerService
                .Setup(x => x.DeleteFile(It.IsAny<string>()));
            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                mockFileManagerService.Object);
            
            projectsFilesService.DeleteProjectFile(
                _dbContext.Projects.First(x => x.Id == 1), 
                _dbContext.Tasks.First(x => x.Id == 1),
                "test.png");
            
            Assert.Null(_dbContext.ProjectTasksFiles.FirstOrDefault(x => x.ProjectId == 1 && x.TaskId == 1 && x.FileName == "test.png"));
            
            CleanUp();
        }
        
        [Fact]
        public void DeleteProjectFile_Project_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.DeleteProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1),
                    _dbContext.Boards.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void DeleteProjectFile_Board_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            
            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.DeleteProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        [Fact]
        public void DeleteProjectFile_Task_FileNotFoundInDb_ThrowsCustomFileNotFoundException()
        {
            SetupDbContext();

            var projectsFilesService = new ProjectsFilesService(
                _dbContext, 
                SetupMockConfigService().Object, 
                SetupMockProjectSecurityService().Object, 
                SetupMockFileManagerService().Object);
            
            Assert.Throws<CustomFileNotFoundException>(() => 
                projectsFilesService.DeleteProjectFile(
                    _dbContext.Projects.First(x => x.Id == 1),
                    _dbContext.Tasks.First(x => x.Id == 1),
                    "FILE NOT EXIST.TEST"));
            
            CleanUp();
        }
        
        private void CleanUp()
        {
            _dbContext?.Database.EnsureDeleted();
            _dbContext?.Dispose();
        }

        private static Mock<IConfigService> SetupMockConfigService()
        {
            var configService = new Mock<IConfigService>();
            configService.Setup(x => x.MaxProjectFileSizeInMb).Returns(1024);

            return configService;
        }
        
        private static Mock<IProjectSecurityService> SetupMockProjectSecurityService()
        {
            var projectSecurityService = new Mock<IProjectSecurityService>();
            projectSecurityService.Setup(x => x.CheckAccess(It.IsAny<Project>(), It.IsAny<ProjectRoles>())).Verifiable();

            return projectSecurityService;
        }
        
        private static Mock<IFileManagerService> SetupMockFileManagerService()
        {
            var fileManagerService = new Mock<IFileManagerService>();

            return fileManagerService;
        }
        
        private void SetupDbContext()
        {
            _dbContext = TestDbUtils.SetupTestDbContextInMemory();
            
            _dbContext.Accounts.AddRange(GetTestAccountList());
            _dbContext.Projects.AddRange(GetTestProjectsList());
            _dbContext.Boards.AddRange(GetTestBoardsList());
            _dbContext.Tasks.AddRange(GetTestTasksList());
            _dbContext.ProjectAccountsRoles.AddRange(GetTestProjectsAccountRoles());
            _dbContext.SaveChanges();
            
            _dbContext.Boards.First(x => x.Id == 1).Project = _dbContext.Projects.First(x => x.Id == 1);
            _dbContext.Boards.First(x => x.Id == 2).Project = _dbContext.Projects.First(x => x.Id == 2);
            
            _dbContext.Tasks.First(x => x.Id == 1).Board = _dbContext.Boards.First(x => x.Id == 1);
            _dbContext.Tasks.First(x => x.Id == 2).Board = _dbContext.Boards.First(x => x.Id == 2);
            _dbContext.SaveChanges();
        }
        
        private static IEnumerable<Account> GetTestAccountList()
        {
            return new List<Account>()
            {
                new Account() {Id = 1, Email = "test1@test.ru", Username = "test1", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 2, Email = "test2@test.ru", Username = "test2", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 3, Email = "test3@test.ru", Username = "test3", IsBanned = false, Salt = new byte[64]},
                new Account() {Id = 4, Email = "test4@test.ru", Username = "test4", IsBanned = false, Salt = new byte[64]},
            };
        }
        
        private static IEnumerable<Project> GetTestProjectsList()
        {
            return new List<Project>()
            {
                new Project() {Id = 1, Title = "Test project1"},
                new Project() {Id = 2, Title = "Test project2"},
            };
        }
        
        private static IEnumerable<Board> GetTestBoardsList()
        {
            return new List<Board>()
            {
                new Board() {Id = 1, Title = "Test board1", PrioritySort = 0},
                new Board() {Id = 2, Title = "Test board2", PrioritySort = 0},
            };
        }
        
        private static IEnumerable<TaskM> GetTestTasksList()
        {
            return new List<TaskM>()
            {
                new TaskM() {Id = 1, Title = "Test task1", PrioritySort = 0, Description = "", Priority = TaskPriority.None},
                new TaskM() {Id = 2, Title = "Test task2", PrioritySort = 0, Description = "", Priority = TaskPriority.None},
            };
        }
        
        private static IEnumerable<ProjectFile> GetProjectFilesList()
        {
            return new List<ProjectFile>()
            {
                new ProjectFile() {ProjectId = 1, FileName = "test.png"},
                new ProjectFile() {ProjectId = 2, FileName = "test.jpg"},
            };
        }
        
        private static IEnumerable<ProjectBoardFile> GetProjectBoardFilesList()
        {
            return new List<ProjectBoardFile>()
            {
                new ProjectBoardFile() {ProjectId = 1, BoardId = 1, FileName = "test.png"},
                new ProjectBoardFile() {ProjectId = 2, BoardId = 2, FileName = "test.jpg"},
            };
        }
        
        private static IEnumerable<ProjectTaskFile> GetProjectTaskFilesList()
        {
            return new List<ProjectTaskFile>()
            {
                new ProjectTaskFile() {ProjectId = 1, TaskId = 1, FileName = "test.png"},
                new ProjectTaskFile() {ProjectId = 2, TaskId = 2, FileName = "test.jpg"},
            };
        }
        
        private static IEnumerable<ProjectAccountsRoles> GetTestProjectsAccountRoles()
        {
            return new List<ProjectAccountsRoles>()
            {
                new ProjectAccountsRoles() {AccountId = 1, ProjectId = 1, ProjectRole = ProjectRoles.ProjectOwner},
                new ProjectAccountsRoles() {AccountId = 2, ProjectId = 1, ProjectRole = ProjectRoles.ProjectAdmin},
                new ProjectAccountsRoles() {AccountId = 2, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 3, ProjectId = 1, ProjectRole = ProjectRoles.ProjectUpdate},
                new ProjectAccountsRoles() {AccountId = 3, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
                new ProjectAccountsRoles() {AccountId = 4, ProjectId = 1, ProjectRole = ProjectRoles.ProjectMember},
            };
        }
    }
}