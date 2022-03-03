using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarkovSharp.TokenisationStrategies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using twitter_baby_birding.Models;
using TwitterSharp;
using TwitterSharp.Response;
using System.Web;

namespace twitter_baby_birding.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;

        // public HomeController(ILogger<HomeController> logger)
        // {
        //     _logger = logger;
        // }

        private TwitterBabyBirdingContext db;
        public HomeController(TwitterBabyBirdingContext context)
        {
            db = context;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.celebs = db.Users;
            return View("Index");
        }

        [HttpPost("barf")]
        public async Task<IActionResult> Generate(TwitterHandle username)
        {
            if(username.Handle == null){
                // Check if they actually entered anything
                ModelState.AddModelError("Handle", "This twitter account was not found.");
                ViewBag.celebs = db.Users;
                return View("Index");
            }
            // Get the user
            TwitterSharp.Response.RUser.User user = await TweetFetcher.GetUser(username.Handle);

            if(user == null){
                //It errored out...
                ModelState.AddModelError("Handle", "This twitter account was not found.");
                return View("Index");
            }

            TwitterSharp.Response.RTweet.Tweet[] TweetArr = await TweetFetcher.FindByHandle(user);
            for(int i = 0; i < TweetArr.Length; i++)
            {
                //Then here in your console you'll be able to see the text output of each tweet gotten
                Console.WriteLine(TweetArr[i].Text);
            }
            // Format tweets as training data
            string[] tweets = TweetArr.Where(t => !t.Text.StartsWith("RT @")).Select(t => t.Text).ToArray();

            // Create a new model
            var model = new StringMarkov(1);

            // Train the model
            model.Learn(tweets);

            // Create some permutations
            List<string> barf = new List<string>();
            barf.Add(model.Walk().First());
            barf.Add(HttpUtility.UrlEncode(barf[0]));

            // string barf = model.Walk().First();

            // Pass generated tweet to a ViewModel

            return View("Generate", barf);
        }

        [HttpGet("{celeb}")]
        public IActionResult Celeb(string celeb)
        {

            ViewBag.celeb = celeb;
            // Create a new model
            var model = new StringMarkov(2);

            var tweets = db.Tweets.Where(t=>t.Author == celeb).Select(t => t.Content);
            // Train the model
            model.Learn(tweets);

            // Create some permutations
            // string barf = model.Walk().First();
            List<string> barf = new List<string>();
            barf.Add(model.Walk().First());
            barf.Add(HttpUtility.UrlEncode(barf[0]));

            // Pass generated tweet to a ViewModel

            return View("GenerateCeleb", barf);
        }

        [HttpGet("about")]
        public ViewResult About()
        {
            return View("About");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
