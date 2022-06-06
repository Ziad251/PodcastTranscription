using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace transcription_project.WebApp.Services
{
    public interface ISpeechTranscriber
    {
        Task TranscribeConversationsAsync(string conversationWaveFile, string subscriptionKey, string region);
        Task TranscribeConversationsWithParticipantsAsync(string conversationWaveFile, string listname, string subscriptionKey, string region);

        void SendTextToClient();
    }
}