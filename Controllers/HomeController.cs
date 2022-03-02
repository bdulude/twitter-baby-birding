using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarkovSharp.TokenisationStrategies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using twitter_baby_birding.Models;
using TwitterSharp;
using TwitterSharp.Response;

namespace twitter_baby_birding.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //So this will fetch all the tweets with a twitterhandle
            TwitterSharp.Response.RUser.User user = await TweetFetcher.GetUser("asdjkehjfkd");
            if(user != null)
            {
                TwitterSharp.Response.RTweet.Tweet[] TweetArr = new TwitterSharp.Response.RTweet.Tweet[0];
                TweetArr = await TweetFetcher.FindByHandle(user);
                for(int i = 0; i < TweetArr.Length; i++)
                {
                    //Then here in your console you'll be able to see the text output of each tweet gotten
                    Console.WriteLine(TweetArr[i].Text);
                }
            }
            return View();
        }

        [HttpPost("barf")]
        public async Task<IActionResult> Generate(TwitterHandle handle)
        {
            ViewBag.handle = handle;
            // Get the tweets for a user
            TwitterSharp.Response.RUser.User user = await TweetFetcher.GetUser(handle.Handle);

            if(user == null){
                //It errored out...
                return View("Index");
            }

            TwitterSharp.Response.RTweet.Tweet[] TweetArr = new TwitterSharp.Response.RTweet.Tweet[0];
            TweetArr = await TweetFetcher.FindByHandle(user);
            for(int i = 0; i < TweetArr.Length; i++)
            {
                //Then here in your console you'll be able to see the text output of each tweet gotten
                Console.WriteLine(TweetArr[i].Text);
            }
            // Format tweets as training data
            string[] tweets = new string[]
            {
                "Frankly, my dear, I don't give a damn.",
                "Mama always said life was like a box of chocolates. You never know what you're gonna get.",
                "Many wealthy people are little more than janitors of their possessions."
            };

            // Create a new model
            var model = new StringMarkov(1);

            // Train the model
            model.Learn(tweets);

            // Create some permutations
            string barf = model.Walk().First();

            // Pass generated tweet to a ViewModel

            return View("Generate", barf);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
