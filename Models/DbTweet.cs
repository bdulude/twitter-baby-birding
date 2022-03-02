using System.ComponentModel.DataAnnotations;

namespace twitter_baby_birding.Models
{
    public class DbTweet
    {
        [Key]
        public int Id {get; set;}
        public string Author {get; set;}
        public string Content {get; set;}
        public string Date_time {get; set;}
    }
}