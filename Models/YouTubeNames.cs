using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace transcription_project.WebApp.Models
{
    public class YouTubeNames
    {
        
        [Key]
        public int id {get; set;} 
        public string UName { get; set; }

        public string Title { get; set; } 
        public string Thumbnail { get; set; } 

        public string VideoId {get; set;}
        public UserData user { get; set; } 

    }

}






