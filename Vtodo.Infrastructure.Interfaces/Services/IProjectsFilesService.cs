using System.IO;
using Vtodo.Entities.Enums;
using Vtodo.Entities.Models;

namespace Vtodo.Infrastructure.Interfaces.Services
{
    internal interface IProjectsFilesService
    {
        string CheckFile(Stream stream, string fileName, string[] rightExtensions);
        
        FileStream GetProjectFile(Project project, string fileName);
        FileStream GetProjectFile(Project project, TaskM task, string fileName);
        FileStream GetProjectFile(Project project, Board board, string fileName);
        
        string UploadProjectFile(Project project, Stream stream, string extension);
        string UploadProjectFile(Project project, TaskM task, Stream stream, string extension);
        string UploadProjectFile(Project project, Board board, Stream stream, string extension);

        void DeleteProjectFile(Project project, string fileName);
        void DeleteProjectFile(Project project, TaskM task, string fileName);
        void DeleteProjectFile(Project project, Board board, string fileName);
    }
}