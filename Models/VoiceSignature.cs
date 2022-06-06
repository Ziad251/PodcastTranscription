using System.Runtime.Serialization;
/// <summary>
/// Class which defines VoiceSignature as specified under https://aka.ms/cts/signaturegenservice.
/// </summary>
namespace transcription_project.WebApp.Models
{


        [DataContract]
        public class VoiceSignature
        {
            [DataMember]
            public string Status { get; private set; }

            [DataMember]
            public VoiceSignatureData Signature { get; private set; }

            [DataMember]
            public string Transcription { get; private set; }
        }

}