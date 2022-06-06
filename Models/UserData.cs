using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace transcription_project.WebApp.Models
{
    public class UserData 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public string Email { get; set; } 
        public ICollection<Container> Containers { get; set; }

        public ICollection<YouTubeNames> YouTubeNames { get; set; }
    } 

}






