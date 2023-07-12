using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Vtodo.Infrastructure.Interfaces.Services;

namespace Vtodo.Infrastructure.Implementation.Services
{
    public class FileManagerService : IFileManagerService
    {
        public string GetBaseFolderPath()
        {
            return Path.GetFullPath(Path.Join(Assembly.GetExecutingAssembly().Location, @"../../VtodoData/"));
        }

        public FileStream? OpenFile(string path)
        {
            return !File.Exists(path) ? null : new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        
        public async Task<string> SaveFile(Stream stream, string path, string extension)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var randomName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
            var filePath = Path.Join(path, randomName);
            
            await using var fs = File.Create(filePath);
            await stream.CopyToAsync(fs);

            return randomName;
        }
        
        public void DeleteFile(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}