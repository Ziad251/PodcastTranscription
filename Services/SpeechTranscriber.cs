using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Transcription;
using Serilog;
using transcription_project.WebApp.Models;
<<<<<<< HEAD
using transcription_project.WebApp.Extensions;
=======
>>>>>>> cfca44a87104f08a8c3d7d53b067cc3192fea55e

namespace transcription_project.WebApp.Services
{

    public class SpeechTranscriber : ISpeechTranscriber
    {
        private UserDbContext _context;
        public IAudioStreamReader _audioStreamReader;
        private HttpContext _httpContext;

        private string result { get; set; }

        public SpeechTranscriber(UserDbContext context, IAudioStreamReader audioStreamReader, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _audioStreamReader = audioStreamReader;
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversationWaveFile"></param>
        /// <param name="listname"></param>
        /// <param name="subscriptionKey"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public async Task TranscribeConversationsAsync(string conversationWaveFile, string subscriptionKey, string region)
        {

            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SetProperty("ConversationTranscriptionInRoomAndOnline", "true");
            config.SetProperty("DifferentiateGuestSpeakers", "true");
            var stopRecognition = new TaskCompletionSource<int>();


            // Create an audio stream from a wav file or from the default microphone if you want to stream live audio from the supported devices
            using (var audioInput = _audioStreamReader.OpenWavFile(conversationWaveFile))
            {

                var meetingID = Guid.NewGuid().ToString();
                using (var conversation = await Conversation.CreateConversationAsync(config, meetingID))
                {
                    // Create a conversation transcriber using audio stream input
                    using (var conversationTranscriber = new ConversationTranscriber(audioInput))
                    {

                        // Subscribe to events

                        conversationTranscriber.Transcribing += (s, e) =>
                        {
                            result = $"TRANSCRIBING: Text={e.Result.Text} SpeakerId={e.Result.UserId}";
                        };

                        conversationTranscriber.Transcribed += (s, e) =>
                        {
                            if (e.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                result = $"TRANSCRIBED: Text={e.Result.Text} SpeakerId={e.Result.UserId}";
                            }
                            else if (e.Result.Reason == ResultReason.NoMatch)
                            {
                                result = $"NOMATCH: Speech could not be recognized.";
                            }
                            SendTextToClient();
                        };

                        conversationTranscriber.Canceled += (s, e) =>
                        {
                            result = $"CANCELED: Reason={e.Reason}";

                            if (e.Reason == CancellationReason.Error)
                            {
                                Log.Information($"CANCELED: ErrorCode={e.ErrorCode}");
                                Log.Information($"CANCELED: ErrorDetails={e.ErrorDetails}");
                                Log.Information($"CANCELED: Did you update the subscription info?");
                                stopRecognition.TrySetResult(0);
                            }
                            SendTextToClient();
                        };

                        conversationTranscriber.SessionStarted += (s, e) =>
                        {
                            result = "session started";
                            SendTextToClient();
                        };

                        conversationTranscriber.SessionStopped += (s, e) =>
                        {
                            result = "close";
                            stopRecognition.TrySetResult(0);
                            SendTextToClient();
                        };

                        // Join to the conversation.
                        await conversationTranscriber.JoinConversationAsync(conversation);


                        // Starts transcribing of the conversation. Uses StopTranscribingAsync() to stop transcribing when all participants leave.
                        await conversationTranscriber.StartTranscribingAsync().ConfigureAwait(false);

                        // Waits for completion.
                        // Use Task.WaitAny to keep the task rooted.
                        Task.WaitAny(new[] { stopRecognition.Task });
                        await conversationTranscriber.StopTranscribingAsync().ConfigureAwait(false);

                    }
                }
            }
        }
        public async Task TranscribeConversationsWithParticipantsAsync(string conversationWaveFile, string listname, string subscriptionKey, string region)
        {

            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SetProperty("ConversationTranscriptionInRoomAndOnline", "true");
            config.SetProperty("DifferentiateGuestSpeakers", "true");
            var stopRecognition = new TaskCompletionSource<int>();


            // Create an audio stream from a wav file or from the default microphone if you want to stream live audio from the supported devices
            using (var audioInput = _audioStreamReader.OpenWavFile(conversationWaveFile))
            {

                var meetingID = Guid.NewGuid().ToString();
                using (var conversation = await Conversation.CreateConversationAsync(config, meetingID))
                {
                    // Create a conversation transcriber using audio stream input
                    using (var conversationTranscriber = new ConversationTranscriber(audioInput))
                    {

                        // Subscribe to events

                        conversationTranscriber.Transcribing += (s, e) =>
                        {
                            result = $"TRANSCRIBING: Text={e.Result.Text} SpeakerId={e.Result.UserId}";
                        };

                        conversationTranscriber.Transcribed += (s, e) =>
                        {
                            if (e.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                result = $"TRANSCRIBED: Text={e.Result.Text} SpeakerId={e.Result.UserId}";
                            }
                            else if (e.Result.Reason == ResultReason.NoMatch)
                            {
                                result = $"NOMATCH: Speech could not be recognized.";
                            }
                            SendTextToClient();
                        };

                        conversationTranscriber.Canceled += (s, e) =>
                        {
                            result = $"CANCELED: Reason={e.Reason}";

                            if (e.Reason == CancellationReason.Error)
                            {
                                Log.Information($"CANCELED: ErrorCode={e.ErrorCode}");
                                Log.Information($"CANCELED: ErrorDetails={e.ErrorDetails}");
                                Log.Information($"CANCELED: Did you update the subscription info?");
                                stopRecognition.TrySetResult(0);
                            }
                            SendTextToClient();
                        };

                        conversationTranscriber.SessionStarted += (s, e) =>
                        {
                            result = "session started";
                            SendTextToClient();
                        };

                        conversationTranscriber.SessionStopped += (s, e) =>
                        {
                            result = "close";
                            stopRecognition.TrySetResult(0);
                            SendTextToClient();
                        };

                        var container = _context.Containers.Where(c => c.containerName == listname).Select(c => c).ToList().FirstOrDefault();

                        var Participants = _context.Participants
                                                    .Where(p => p.container == container)
                                                    .Select(p => new
                                                    {
                                                        Name = p.personName.Replace(".wav", ""),
                                                        Signature = p.voiceData,
                                                    })
                                                    .ToArray();

                        foreach (var participant in Participants)
                        {
                            // Add participants to the conversation.
                            // Voice signature needs to be in the following format:
                            // { "Version": <Numeric value>, "Tag": "string", "Data": "string" }
                            var languageForUser = "en-US"; // For example "en-US"
                            var speaker = Microsoft.CognitiveServices.Speech.Transcription.Participant.From($"{participant.Name}", languageForUser, participant.Signature);
                            await conversation.AddParticipantAsync(speaker);
                        }

                        // Join to the conversation.
                        await conversationTranscriber.JoinConversationAsync(conversation);


                        // Starts transcribing of the conversation. Uses StopTranscribingAsync() to stop transcribing when all participants leave.
                        await conversationTranscriber.StartTranscribingAsync().ConfigureAwait(false);

                        // Waits for completion.
                        // Use Task.WaitAny to keep the task rooted.
                        Task.WaitAny(new[] { stopRecognition.Task });
                        await conversationTranscriber.StopTranscribingAsync().ConfigureAwait(false);

                    }
                }
            }
        }

        public async void SendTextToClient()
        {

            if (result == "close")
            {
                await _httpContext.SSESendEventAsync(
               new SSEEvent("close", new { result = "session stopped.", })
               {
                   Id = "my id",
                   Retry = 1
               }
            );
                return;
            }

            if (result == "") 
            {
                result = "unintelligible";
            }

            await _httpContext.SSESendEventAsync(
               new SSEEvent("event name", new { result })
               {
                   Id = "my id",
                   Retry = 10
               }
            );
        }

    }
}