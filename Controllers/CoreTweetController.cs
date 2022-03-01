using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using twitter_baby_birding.Models;
using CoreTweet;
using CoreTweet.Streaming;
using TwitterSharp;
using TwitterSharp.Request.AdvancedSearch;

namespace twitter_baby_birding.Controllers
{
    public class CoreTweetController : Controller
    {
        private readonly ILogger<CoreTweetController> _logger;

        public CoreTweetController(ILogger<CoreTweetController> logger)
        {
            _logger = logger;
        }
        [HttpGet("Tweet/{twitterHandle}")]
        public IActionResult Index(string twitterHandle)
        {
            
            return new JsonResult(new{data = Requests(twitterHandle)});
        }

        public async Task<TwitterSharp.Response.RTweet.Tweet[]> Requests(string twitterHandle)
        {
            var client = new TwitterSharp.Client.TwitterClient(TwitterKeys.Bearer);
            var user = await client.GetUserAsync(twitterHandle);
            Console.WriteLine(user.Id);
            var tweets = await client.GetTweetsFromUserIdAsync(user.Id, new TweetOption[] { TweetOption.Created_At }, null, new MediaOption[] { MediaOption.Url });
            for (int i = 0; i < tweets.Length; i++)
            {
                var tweet = tweets[i];
                Console.WriteLine($"Tweet n°{i}");
                Console.WriteLine(tweet.Text);
                if (tweet.Attachments?.Media?.Any() ?? false)
                {
                    Console.WriteLine("\nImages:");
                    Console.WriteLine(string.Join("\n", tweet.Attachments.Media.Select(x => x.Url)));
                }
                Console.WriteLine("\n");
            }
            return tweets; 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
