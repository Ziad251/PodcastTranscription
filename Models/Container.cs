using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace transcription_project.WebApp.Models
{
    public class Container
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid containerUid {get; set;} 
        public string containerName { get; set; } 
        public UserData user { get; set; } 
        public ICollection<Participant> Participants { get; set; } 

    }

}






