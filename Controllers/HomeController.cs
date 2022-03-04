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
                ModelState.AddModelError("Handle", "You must enter a username.");
                ViewBag.celebs = db.Users;
                return View("Index");
            }
            // Get the user
            TwitterSharp.Response.RUser.User user = await TweetFetcher.GetUser(username.Handle);

            if(user == null){
                //It errored out...
                ModelState.AddModelError("Handle", "This twitter account was not found.");
                ViewBag.celebs = db.Users;
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

        [HttpGet("celeb/{celeb}")]
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

        [HttpGet("MultiTweet")]
        public IActionResult MultiTweet()
        {
            ViewBag.UsersCount = 2;
            return View();    
        }


        [HttpPost("multiBarf")]
        public async Task<IActionResult> GenerateMulti(TwitterHandle username)
        {
            //Preliminary Check to see if any of the users are null
            for (int i = 0; i < username.MultiHandle.Count; i++)
            {
                string user = username.MultiHandle[i];
                if(user== null)
                {
                    ModelState.AddModelError($"MultiHandle[{i}]", "There is an empty field!");
                    return View("MultiTweet");
                }
                else if (user.Length > 15)
                {
                    ModelState.AddModelError($"MultiHandle[{i}]", $"{user} is too long! Each username must be less than 15 characters");
                    return View("MultiTweet");
                }
            }
            //Setting the variables to be more readable

            int NumberOfUsers = username.MultiHandle.Count;
            List<string> UserHandles = username.MultiHandle;
            TwitterSharp.Response.RUser.User[] UsersArray= new TwitterSharp.Response.RUser.User[NumberOfUsers];
            
            for (int i = 0; i < username.MultiHandle.Count; i++)
            {
                //Set the User
                UsersArray[i] = await TweetFetcher.GetUser(username.MultiHandle[i]);
                if(UsersArray[i] == null)
                {
                    //If the User is null add an error to the Model
                    ModelState.AddModelError($"MultiHandle[{i}]", "This username is not associated with a valid twitter account.");
                }
            }
            //If Any of the Users are null Then the state is invalid
            ViewBag.UsersHandles = UserHandles;
            if(!ModelState.IsValid)
            {
                    ViewBag.UsersCount = UserHandles.Count;
                    return View("MultiTweet",username);
            }

            // Determine how much data per user
            // int tweetsPerUser = (int)MathF.Floor(100/NumberOfUsers);
            int tweetsPerUser = 100;

            //Create an array to hold all the gotten tweets
            TwitterSharp.Response.RTweet.Tweet[] TweetArr = new TwitterSharp.Response.RTweet.Tweet[0];
            foreach(TwitterSharp.Response.RUser.User User in UsersArray)
            {
                //Get Tweets for that user and concat them onto the newTweets
                TwitterSharp.Response.RTweet.Tweet[] newTweets = await TweetFetcher.FindByHandle(User,tweetsPerUser);
                TweetArr = TweetArr.Concat(newTweets).ToArray();
            }

            string[] tweets = TweetArr.Where(t => !t.Text.StartsWith("RT @")).Select(t => t.Text).ToArray();

            // Create a new model
            var model = new StringMarkov(1);

            // Train the model
            model.Learn(tweets);

            // Create some permutations
            List<string> barf = new List<string>();
            barf.Add(model.Walk().First());
            barf.Add(HttpUtility.UrlEncode(barf[0]));

            // Pass generated tweet to a ViewModel

            return View("GenerateChimera", barf);
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [HttpGet("chaos/celebs")]
        public IActionResult Chaos()
        {
            // Create a new model
            var model = new StringMarkov(1);

            List<string> tweetList = new List<string>();
            
            List<string> Celebs =  db.Users.Select(t => t.Name).ToList();
            
            ViewBag.UsersHandles = Celebs; 
            Random rand = new Random();
            foreach (string Celeb in Celebs)
            {
                var tweets = db.Tweets.Where(t=>t.Author == Celeb).Select(t => t.Content).ToArray();
                //Get 100 Random tweets from a Celeb
                for (int i = 0; i < 100; i++)
                {
                    tweetList.Add(tweets[rand.Next(0,tweetList.Count)]);
                }
            }
            // Train the model
            model.Learn(tweetList);

            // Create some permutations
            // string barf = model.Walk().First();
            List<string> barf = new List<string>();
            barf.Add(model.Walk().First());
            barf.Add(HttpUtility.UrlEncode(barf[0]));

            // Pass generated tweet to a ViewModel

            return View("GenerateChaos", barf);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
