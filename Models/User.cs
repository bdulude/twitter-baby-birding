using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace twitter_baby_birding.Models
{
    public class User
    {
        [Key]
        public string Name {get; set;}
        public List<Tweet> Tweets {get; set;}
    }
}