using System.Threading.Tasks;
using System.IO;
using transcription_project.WebApp.Models;

namespace transcription_project.WebApp.Services
{
    public interface IBlobService
    {
        public Task<Stream> GetBlobAsync(string name, Container containerName);
        public Task UploadFileBlobAsync(Stream cont, string fileName, Container container);
    }
}