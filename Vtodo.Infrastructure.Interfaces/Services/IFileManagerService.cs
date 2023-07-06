using System.IO;
using System.Threading.Tasks;

namespace Vtodo.Infrastructure.Interfaces.Services
{
    public interface IFileManagerService
    {
        string GetBaseFolderPath();
        FileStream? OpenFile(string path);
        Task<string> SaveFile(Stream stream, string path, string extension);
        void DeleteFile(string path);
    }
}