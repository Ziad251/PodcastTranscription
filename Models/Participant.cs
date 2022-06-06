using System.ComponentModel.DataAnnotations;

namespace transcription_project.WebApp.Models
{
    public class Participant
    {

        [Key]
        public int participantId { get; set; } 
        public string personName { get; set; } 
        public string voiceData { get; set; } 

        public Container container {get; set;}

    }

}






