using System.Runtime.Serialization;
/// <summary>
/// Class which defines VoiceSignatureData which is used when creating/adding participants
/// </summary>
namespace transcription_project.WebApp.Models
{

        [DataContract]
        public class VoiceSignatureData
        {
            public VoiceSignatureData()
            { }

            public VoiceSignatureData(int version, string tag, string data)
            {
                this.Version = version;
                this.Tag = tag;
                this.Data = data;
            }

            [DataMember]
            public int Version { get; private set; }

            [DataMember]
            public string Tag { get; private set; }

            [DataMember]
            public string Data { get; private set; }
        }

}