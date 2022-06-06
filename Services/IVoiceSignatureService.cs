using System.Threading.Tasks;
using transcription_project.WebApp.Models;

namespace transcription_project.WebApp.Services
{
    public interface IVoiceSignatureService
    {
        Task<VoiceSignature> CreateVoiceSignatureFromVoiceSampleAsync(byte[] fileBytes, string subscriptionKey, string region);
    }
}