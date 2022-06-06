using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using transcription_project.WebApp.Extensions;
using transcription_project.WebApp.Models;
using transcription_project.WebApp.Services;
using transcription_project.WebApp.Utils;
using System.Collections.Generic;

namespace transcription_project.WebApp.Services
{

    public class RepositoryService : IRepositoryService
    {
        private UserDbContext _context;
        private IConfiguration _configuration;

        public RepositoryService(UserDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<UserData> FindUserByEmail(string email)
        {
            var user = _context.UserDatas.Where(u => u.Email == email).Select(u => u).FirstOrDefault();

            if (user == null)
            {
                var newuser = new UserData()
                {
                    Email = email,
                };
                _context.UserDatas.Add(newuser);
                _context.SaveChanges();
                return newuser;
            }
            return user;

        }

        public async Task<Container> FindContainerByName(string listName, UserData user)
        {
            IQueryable<Container> containersList = _context.Containers.Where(c => c.containerName == listName && c.user == user);
            return await containersList.FirstAsync();

        }

        // searches the db_context instance using the id from ClaimsProvider and return a UserData.
        public async Task<UserData> FindUserByGuidString(string guidString)
        {
            Guid userGuid = Guid.Parse(guidString);
            var user = _context.UserDatas.FindAsync(userGuid);
            return await user;
        }

        public async Task<string[]> GetAllContainerNames(UserData user)
        {
            IQueryable<Container> containersList = _context.Containers.Where(c => c.containerName != null && c.user == user);
            return await containersList.Select(c => c.containerName).ToArrayAsync();
        }

        public async Task<UserData> AddUser(string email)
        {
            var newuser = new UserData()
            {
                Email = email,
            };
            _context.UserDatas.Add(newuser);
            return newuser;
        }

        public async Task AddContainer(Container newContainer)
        {
            // Use a separate db_context instance to handle the parallel operation conflicts
            var newDbContext = new DbExtension(_configuration);
            var optionsBuilder = newDbContext.BuildOptions();
            using (UserDbContext context = new UserDbContext(optionsBuilder.Options))
            {
                bool saveFailed;
                context.Containers.Add(newContainer);
                do
                {
                    saveFailed = false;
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        saveFailed = true;
                        HandleException.Handler(e);
                    }
                } while (saveFailed);
            }
        }

        public async Task AddYouTubeTitle(YouTubeNames newTitle)
        {
            _context.YouTubeTitles.Add(newTitle);
        }

        public async Task<Object> FindAllYouTubeTitles(UserData user)
        {
            IQueryable<YouTubeNames> youtubeTitlesList = _context.YouTubeTitles.Where(y => y.Title != null && y.user == user);
            return await youtubeTitlesList.Select(y => new{Yid=y.id, Title=y.Title, UName=y.UName, Thumbnail=y.Thumbnail, VideoId=y.VideoId}).OrderBy(y => y.Yid).ToListAsync();
        }

        public async Task AddParticipant(string containerName, UserData user, string pname, VoiceSignature voiceSignature)
        {
            var newDbContext = new DbExtension(_configuration);
            var optionsBuilder = newDbContext.BuildOptions();
            using (UserDbContext context = new UserDbContext(optionsBuilder.Options))
            {

                // Find the exact container specified and add to it the participants.
                IQueryable<Container> containersList = _context.Containers.Where(c => c.containerName == containerName && c.user == user);
                Container container = containersList.First();
                // convert participant's signature to Json and set it to the new instance property.
                var signatureJson = JsonConvert.SerializeObject(voiceSignature.Signature);
                var p = new Participant
                {
                    personName = pname,
                    voiceData = signatureJson,
                    container = container,
                };
                context.Participants.Add(p);
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        saveFailed = true;
                        HandleException.Handler(e);
                    }
                } while (saveFailed);
                
                await context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }



    }

}
