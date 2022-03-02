using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using twitter_baby_birding.Models;
using TwitterSharp;
using TwitterSharp.Request.AdvancedSearch;

namespace twitter_baby_birding.Controllers
{
    public class TweetController : Controller
    {
        private readonly ILogger<TweetController> _logger;

        public TweetController(ILogger<TweetController> logger)
        {
            _logger = logger;
        }
        [HttpGet("Tweet/{twitterHandle}")]
        public IActionResult Index(string twitterHandle)
        {
            return new JsonResult(new{data = FindByHandle(twitterHandle)});
        }

        public async Task<TwitterSharp.Response.RTweet.Tweet[]> FindByHandle(string twitterHandle)
        {
            var client = new TwitterSharp.Client.TwitterClient(TwitterKeys.Bearer);
            var user = await client.GetUserAsync(twitterHandle);
            Console.WriteLine(user.Id);
            var tweets = await client.GetTweetsFromUserIdAsyncCount(user.Id, new TweetOption[] { TweetOption.Created_At }, null, new MediaOption[] { MediaOption.Url },100);
            // for (int i = 0; i < tweets.Length; i++)
            // {
            //     var tweet = tweets[i];
            //     Console.WriteLine($"Tweet n°{i}");
            //     Console.WriteLine(tweet.Text);
            //     if (tweet.Attachments?.Media?.Any() ?? false)
            //     {
            //         Console.WriteLine("\nImages:");
            //         Console.WriteLine(string.Join("\n", tweet.Attachments.Media.Select(x => x.Url)));
            //     }
            //     Console.WriteLine("\n");
            // }
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

    /*
    <summary>
    The static version that doesn't have an end point and is used to fetch
    a List of tweets.
    </summary>
    */
    public static class TweetFetcher
    {
        // Summary:
        //     Takes in a Twitter Handle to get a user
        //     Then takes the user's ID and finds their tweets.
        //
        // Parameters:
        //   TwitterHandle:
        //     The Handle of the user, it's usually found underneath their displayname
        //      With an @ symbol
        //
        // Returns:
        //     An asychronous call that returns a listn array of Tweets
        public static async Task<TwitterSharp.Response.RTweet.Tweet[]> FindByHandle(string TwitterHandle)
        {
            var client = new TwitterSharp.Client.TwitterClient(TwitterKeys.Bearer);
            var user = await client.GetUserAsync(TwitterHandle);
            Console.WriteLine(user.Id);
            var tweets = await client.GetTweetsFromUserIdAsyncCount(user.Id, new TweetOption[] { TweetOption.Created_At }, null, new MediaOption[] { MediaOption.Url },100);

            return tweets; 
        }
    }
    
    
}
