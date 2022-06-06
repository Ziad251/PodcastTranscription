using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using transcription_project.WebApp.Models;

namespace transcription_project.WebApp.Services
{
    public interface IRepositoryService 
    {
        Task<Container> FindContainerByName(string listName, UserData user);
        Task<UserData> FindUserByGuidString(string guidString);
        Task<UserData> FindUserByEmail(string email);
        Task<string[]> GetAllContainerNames(UserData user);
        
        Task AddContainer(Container newContainer);

        Task AddYouTubeTitle (YouTubeNames newTitle);
        Task<Object> FindAllYouTubeTitles(UserData user);
        Task AddParticipant(string containerName, UserData user, string name, VoiceSignature voiceSignature);
        Task<int> SaveAsync();
        Task<UserData> AddUser(string email);
    }

}
