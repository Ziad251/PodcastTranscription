using System;
using Microsoft.AspNetCore.Mvc;
using transcription_project.WebApp.Services;
using transcription_project.WebApp.Models;
using transcription_project.WebApp.Extensions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using transcription_project.WebApp.Utils;
using VideoLibrary;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using System.Linq;

namespace transcription_project.WebApp.Controllers
{
    [Route("api/{action}")]
    public class ApiController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;
        private readonly IVoiceSignatureService _voiceSignatureService;
        private readonly ISpeechTranscriber _speechTranscriber;
        private readonly IGetClaimsProvider _claims;
        private readonly IRepositoryService _repository;
        private string _dir;
        private VideoDownloader _videoDownloader;

        public ApiController(IBlobService blobService,
        IConfiguration Configuration,
        IVoiceSignatureService vcservice,
        ISpeechTranscriber transcriber,
        IGetClaimsProvider claims,
        IRepositoryService repository,
        IWebHostEnvironment env,
        VideoDownloader videoDownloader)
        {
            _blobService = blobService;
            _configuration = Configuration;
            _voiceSignatureService = vcservice;
            _speechTranscriber = transcriber;
            _claims = claims;
            _repository = repository;
            _dir = env.WebRootPath;
            _videoDownloader = videoDownloader;

        }


        [Authorize]
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> RegisterParticipants(CancellationToken cancellationToken)
        {

            try
            {
                // Represents the collection of files sent with the HttpRequest.
                IFormFileCollection files = HttpContext.Request.Form.Files;
                string containerName = files[0].Name;
                UserData user = await _repository.FindUserByEmail(_claims.Email);

                foreach (var file in files)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] bytes = memoryStream.ToArray();
                        string subscriptionKey = _configuration.GetValue<string>("SpeechService:Key");
                        string region = _configuration.GetValue<string>("SpeechService:Region");

                        var voiceSignature = await _voiceSignatureService.CreateVoiceSignatureFromVoiceSampleAsync(bytes, subscriptionKey, region);

                        var newContainer = new Container
                        {
                            containerName = containerName,
                            user = user,
                        };
                        await _repository.AddContainer(newContainer);

                        if (voiceSignature != null && voiceSignature.Status == "OK")
                        {
                            await _repository.AddParticipant(containerName, user, file.FileName, voiceSignature);
                        }

                        if (voiceSignature == null)
                        {
                            return BadRequest();
                        }
                    }
                }

                return Ok();
            }

            catch (Exception e)
            {
                HandleException.Handler(e);
            }

            await _repository.SaveAsync();
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LookUpYouTube(CancellationToken cancellationToken)
        {
            var key = "AIzaSyCHztzEdEuHzSoe9uUrdXyqOuQrqASwhBY";
            var json = "";
            string search = HttpContext.Request.Query["search"].ToString();

            using (HttpClient client = new HttpClient())
            {

                var response = await client.GetAsync($"https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=5&q={search}&key={key} ");
                var res = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<YouTubeSearchResult>(res);

                var items = result.items;
                json = JsonSerializer.Serialize(items);
            }


            return Ok(json);

        }

        [HttpGet]
        public async Task<IActionResult> AddVideo(CancellationToken cancellationToken)
        {
            string id = HttpContext.Request.Query["id"].ToString();
            string thumb = HttpContext.Request.Query["thumb"].ToString();
            string title = HttpContext.Request.Query["title"].ToString();

            string pattern = "^((?!-)[A-Za-z0-9-]{1, 63}(?<!-)\\.)+[A-Za-z]{2, 6}$";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string newName = rgx.Replace(title, replacement);
            var email = User.Claims.SingleOrDefault(x => x.Type == "emails")?.Value;
            UserData user = await _repository.FindUserByEmail(email);
            var newTitle = new YouTubeNames
            {
                Title = title,
                UName = newName,
                Thumbnail = thumb,
                VideoId = id,
                user = user,
            };
            await _repository.AddYouTubeTitle(newTitle);
            await _repository.SaveAsync();

            return Json(new { Url = "Results" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideoNames()
        {
            var email = User.Claims.SingleOrDefault(x => x.Type == "emails")?.Value;
            UserData user = await _repository.FindUserByEmail(email);
            var youTubeNames = await _repository.FindAllYouTubeTitles(user);
            if (youTubeNames != null)
            {
                var json = JsonSerializer.Serialize(youTubeNames);
                return Ok(json);
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetFile(CancellationToken cancellationToken)
        {

            string id = HttpContext.Request.Query["id"].ToString();
            string uname = HttpContext.Request.Query["fileName"].ToString();
            _videoDownloader.PullVideoFromYouTube(id).GetAwaiter().GetResult();
            string inputFile = Path.Combine(_dir, uname + ".mp4");
            string outputPath = Path.ChangeExtension(Path.Combine(_dir, uname), ".wav");
            string arguments = $"-i {inputFile} -vn -acodec pcm_s16le -ar 44100 -ac 2 {outputPath}";
            //Set directory where app should look for FFmpeg
            FFmpeg.SetExecutablesPath("C:/Users/HP/Desktop/both/workspace/transcription-project.WebApp/bin/Debug/net6.0/");
            //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
            // await Xabe.FFmpeg.Downloader.FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            var Conversions = new Conversion();
            var conversionResult = await Conversions.Start(arguments);

            // delete file from server after completion 
            // System.IO.File.Delete(inputFile);
            string returnUrl = $"Home/RequestTranscription/{id}" ;
            return Json(new { Url = returnUrl });
        }




        [Authorize]
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadConversationFile(CancellationToken cancellationToken)
        {
            var file = HttpContext.Request.Form.Files[0];
            UserData user = await _repository.FindUserByEmail(_claims.Email);
            Container container = await _repository.FindContainerByName(file.Name, user);
            string id = "conversation.wav";

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    await _blobService.UploadFileBlobAsync(memoryStream, id, container);
                }
                return Ok("/Home/GetAllContainers");
            }
            catch (Exception e)
            {
                Log.Error($"Upload of Conversation to Az failed.", e.Message);
            }

            return BadRequest();

        }


        [HttpGet("{listName}")]
        public async Task StartTranscription([FromRoute] string listName, CancellationToken cancellationToken)
        {

            string dir = $"{_dir}" + $"\\{listName}" + ".wav";
            string pattern = "^((?!-)[A-Za-z0-9-]{1, 63}(?<!-)\\.)+[A-Za-z]{2, 6}$";
            string replacement = "/";
            Regex rgx = new Regex(pattern);
            string path = rgx.Replace(dir, replacement);

            try
            {
                await HttpContext.SSEInitAsync();
                await HttpContext.SSESendEventAsync(
                    new SSEEvent("event name", new { result = "starting transcription ..." })
                    {
                        Id = "my id",
                        Retry = 10
                    });
                await _speechTranscriber.TranscribeConversationsAsync(path, _configuration.GetValue<string>("SpeechService:Key"), _configuration.GetValue<string>("SpeechService:Region"));
            }
            catch (OperationCanceledException)
            {

            }
            // delete file from server after completion 
            System.IO.File.Delete(path);

        }


        [HttpGet("{listName}")]
        public async Task StartTranscriptionWithCustomAudio([FromRoute] string listName, CancellationToken cancellationToken)
        {
            await HttpContext.SSEInitAsync();
            UserData user = await _repository.FindUserByEmail(_claims.Email);
            Container container;
            try
            {
                container = await _repository.FindContainerByName(listName, user);
            }
            catch (Exception e)
            {
                Log.Error("Container name invalid. No such container in database. ", e.Message);
                await HttpContext.SSESendEventAsync(
                  new SSEEvent("close", new { res = "Error:  Invalid container name. No such container in database.", })
                  {
                      Id = "my id",
                      Retry = 1
                  }
               );
                return;
            }

            //get the conversation wav from our az storage, store it locally and send it of to microsoft.speech.services
            string path = "./media/" + container.containerUid.ToString() + ".wav";
            bool success = true;
            try
            {
                // get the wav file using the Guid property to make sure their won't be any conflicts or abuses.
                using (var conversationAudioFileBlob = await _blobService.GetBlobAsync("conversation.wav", container))
                {
                    using (Stream content = conversationAudioFileBlob)
                    {
                        // Download the audio Stream locally.
                        using (FileStream fs = System.IO.File.Create(path))
                        {
                            content.CopyTo(fs);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error dowloading file from server. ", e.Message);
                success = false;

                await HttpContext.SSESendEventAsync(
                  new SSEEvent("close", new { res = "Error dowloading file from server. Please try again.", })
                  {
                      Id = "my id",
                      Retry = 1
                  }
               );
            }
            if (success)
            {

                try
                {
                    await HttpContext.SSESendEventAsync(
                        new SSEEvent("event name", new { result = "starting transcription ..." })
                        {
                            Id = "my id",
                            Retry = 10
                        });
                    await _speechTranscriber.TranscribeConversationsWithParticipantsAsync(path, listName, _configuration.GetValue<string>("SpeechService:Key"), _configuration.GetValue<string>("SpeechService:Region"));
                }
                catch (OperationCanceledException)
                {

                }
                // delete file from server after completion 
                System.IO.File.Delete(path);
            }

        }
    }
}
