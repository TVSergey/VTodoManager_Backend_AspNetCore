using System;
using System.IO;
using System.Linq;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Exceptions;
using Vtodo.Entities.Models;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    internal class ProjectsFilesService : IProjectsFilesService
    {
        private readonly IDbContext _dbContext;
        private readonly IConfigService _configService;
        private readonly IProjectSecurityService _projectSecurityService;
        private readonly IFileManagerService _fileManagerService;
        
        public ProjectsFilesService(
            IDbContext dbContext,
            IConfigService configService, 
            IProjectSecurityService projectSecurityService,
            IFileManagerService fileManagerService)
        {
            _dbContext = dbContext;
            _configService = configService;
            _projectSecurityService = projectSecurityService;
            _fileManagerService = fileManagerService;
        }

        public string CheckFile(Stream fileStream, string fileName, string[] rightExtensions)
        {
            if (fileStream.Length == 0) throw new InvalidFileException();

            if (fileStream.Length / 1000000 > _configService.MaxProjectFileSizeInMb) throw new InvalidFileException();
            
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension)) throw new InvalidFileException();
            
            if (!rightExtensions.Contains(extension.ToLower())) throw new InvalidFileException();
            
            return extension;
        }

        public FileStream GetProjectFile(Project project, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectMember);
            
            var projectFile = _dbContext.ProjectFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();

            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "ProjectFile", fileName);
            
            var file = _fileManagerService.OpenFile(path);
            if (file == null) throw new CustomFileNotFoundException();

            return file;
        }
        
        public FileStream GetProjectFile(Project project, TaskM task, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectMember);
            
            var projectFile = _dbContext.ProjectTasksFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.TaskId == task.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();

            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Tasks", task.Id.ToString(), fileName);
            
            var file = _fileManagerService.OpenFile(path);
            if (file == null) throw new CustomFileNotFoundException();

            return file;
        }

        public FileStream GetProjectFile(Project project, Board board, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectMember);
            
            var projectFile = _dbContext.ProjectBoardsFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.BoardId == board.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();

            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Boards", board.Id.ToString(), fileName);
            
            var file = _fileManagerService.OpenFile(path);
            if (file == null) throw new CustomFileNotFoundException();

            return file;
        }
        
        public string UploadProjectFile(Project project, Stream stream, string extension)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "ProjectFile");
            var fileName = _fileManagerService.SaveFile(stream, path, extension).Result;
            
            _dbContext.ProjectFiles.Add(new ProjectFile() {ProjectId = project.Id, FileName = fileName});
            _dbContext.SaveChanges();
            
            return fileName;
        }

        public string UploadProjectFile(Project project, TaskM task, Stream stream, string extension)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Tasks", task.Id.ToString());
            var fileName = _fileManagerService.SaveFile(stream, path, extension).Result;

            _dbContext.ProjectTasksFiles.Add(new ProjectTaskFile() {ProjectId = project.Id, TaskId  = task.Id ,FileName = fileName});
            _dbContext.SaveChanges();
            
            return fileName;
        }

        public string UploadProjectFile(Project project, Board board, Stream stream, string extension)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Boards", board.Id.ToString());
            var fileName = _fileManagerService.SaveFile(stream, path, extension).Result;

            _dbContext.ProjectBoardsFiles.Add(new ProjectBoardFile() {ProjectId = project.Id, BoardId  = board.Id ,FileName = fileName});
            _dbContext.SaveChanges();
            
            return fileName;
        }

        public void DeleteProjectFile(Project project, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);

            var projectFile = _dbContext.ProjectFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();
            
            _dbContext.ProjectFiles.Remove(projectFile);
            _dbContext.SaveChanges();
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "ProjectFile", fileName);
            _fileManagerService.DeleteFile(path);
        }

        public void DeleteProjectFile(Project project, TaskM task, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var projectFile = _dbContext.ProjectTasksFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.TaskId == task.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();
            
            _dbContext.ProjectTasksFiles.Remove(projectFile);
            _dbContext.SaveChanges();
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Tasks", task.Id.ToString(), fileName);
            _fileManagerService.DeleteFile(path);
        }
        
        public void DeleteProjectFile(Project project, Board board, string fileName)
        {
            _projectSecurityService.CheckAccess(project, ProjectRoles.ProjectUpdate);
            
            var projectFile = _dbContext.ProjectBoardsFiles.FirstOrDefault(x => x.ProjectId == project.Id && x.BoardId == board.Id && x.FileName == fileName);
            if (projectFile == null) throw new CustomFileNotFoundException();
            
            _dbContext.ProjectBoardsFiles.Remove(projectFile);
            _dbContext.SaveChanges();
            
            var path = Path.Join(_fileManagerService.GetBaseFolderPath(), "Projects", project.Id.ToString(), "Boards", board.Id.ToString(), fileName);
            _fileManagerService.DeleteFile(path);
        }

    }
}