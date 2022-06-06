using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using transcription_project.WebApp.Models;

namespace transcription_project.WebApp.Services
{

    public class VoiceSignatureService : IVoiceSignatureService
    {
        public VoiceSignatureService(HttpClient client)
        {
            _client = client;
        }
        private VoiceSignature result;
        private readonly HttpClient _client;

        public async Task<VoiceSignature> CreateVoiceSignatureFromVoiceSampleAsync(byte[] fileBytes, string subscriptionKey, string region)
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            var content = new ByteArrayContent(fileBytes);
            var response = await _client.PostAsync("https://signature.centralus.cts.speech.microsoft.com/api/v1/Signature/GenerateVoiceSignatureFromByteArray", content);

            // A voice signature contains Version, Tag and Data key values from the Signature json structure from the Response body.
            // Voice signature format example: { "Version": <Numeric string or integer value>, "Tag": "string", "Data": "string" }
            var jsonData = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<VoiceSignature>(jsonData);

            return result;

        }
    }
}