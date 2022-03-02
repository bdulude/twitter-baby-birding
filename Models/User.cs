using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace twitter_baby_birding.Models
{
    public class User
    {
        [Key]
        public string Author {get; set;}
        public List<DbTweet> DbTweets {get; set;}
    }
}