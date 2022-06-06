using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using VideoLibrary;


namespace transcription_project.WebApp.Services
{

    public class VideoDownloader
    {
        private string _dir;
        public VideoDownloader(IWebHostEnvironment env)
        {
            _dir = env.WebRootPath;
        }

        public async Task PullVideoFromYouTube(string id)
        {
            var link = $"https://www.youtube.com/watch?v={id}";
            string newName;
            using (var service = Client.For(YouTube.Default))
            {

                var video = service.GetVideo(link);
                var name = video.FullName;
                string pattern = "[\\s\\-\\–_!',]+";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                newName = rgx.Replace(name, replacement);
                string path = Path.Combine(_dir, newName);
                System.IO.File.WriteAllBytes(path, video.GetBytes());


            }

        }
    }
}