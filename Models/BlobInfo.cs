using System.IO;

namespace transcription_project.WebApp.Models
{
    public class BlobInfo
    {
        public BlobInfo(Stream content, string contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }

        public Stream Content {get;}
        public string ContentType {get;}
    }
}