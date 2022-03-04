using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace twitter_baby_birding.Models
{
    [Keyless]
    public class User
    {
        public string Name {get; set;}
    }
}