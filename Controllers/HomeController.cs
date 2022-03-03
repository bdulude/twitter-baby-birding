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

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("barf")]
        public async Task<IActionResult> Generate(TwitterHandle username)
        {
            // Get the tweets for a user
            Console.WriteLine(username.Handle);
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
            string barf = model.Walk().First();

            // Pass generated tweet to a ViewModel

            return View("Generate", barf);
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
            //Prelimanary Check to see if any of the users are null
            foreach(string user in username.MultiHandle)
            {
                if(user== null)
                {
                    ModelState.AddModelError("MultiHandle", "Error! There is an empty Field!");
                    return View("MultiTweet");
                }
                else if (user.Length > 15)
                {
                    ModelState.AddModelError("MultiHandle", $"Error! {user} is too long! Less than 15 Characters");
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
                    ModelState.AddModelError($"MultiHandle[{i}]", "Error! Handle is not valid!!");
                }
            }
            //If Any of the Users are null Then the state is invalid
            if(!ModelState.IsValid)
            {
                    ViewBag.UsersCount = UserHandles.Count;
                    ViewBag.UsersHandles = UserHandles;
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

            //Filter them out
            string[] tweets = TweetArr.Where(t => !t.Text.StartsWith("RT @")).Select(t => t.Text).ToArray();

            // Create a new model
            var model = new StringMarkov(1);

            // Train the model
            model.Learn(tweets);

            // Create some permutations
            string barf = model.Walk().First();
            Console.WriteLine(barf);
            return View("Generate", barf);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
