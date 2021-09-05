using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Models;
using Tweetinvi.Models;

namespace PS5Bot
{
    public class TwitterReader
    {
        private string CONSUMER_KEY;
        private string CONSUMER_SECRET;
        private string ACCESS_TOKEN;
        private string ACCESS_TOKEN_SECRET;

        private TwitterClient userClient;
        
        public TwitterReader()
        {
            CONSUMER_KEY = "qvTRPHUJpsdewfOs311JQbV6R";
            CONSUMER_SECRET = "hNmkeLqFpMQd704vUxfn6ky675FZ1sJwubY6XNxdoXnY1ckLwX";
            ACCESS_TOKEN = "1403296034885877761-afgIbyZo0N2KA3DwZmZVSFbp6ksYqW";
            ACCESS_TOKEN_SECRET = "QmikqebmPHktvzTJFAISCTxFSPNODCMxhtPwnAWsinYfK";
        }

        public async Task CheckTweets()
        {
            var userClient = new TwitterClient(CONSUMER_KEY,CONSUMER_SECRET , ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
            var user = await userClient.Users.GetAuthenticatedUserAsync();
            
            Console.WriteLine("UserName: " + user);
        }

        // private ITweet[] GetTweets()
        // {
        //     var tweets = new List<ITweet>();
        //
        //     var receivedTweets = Timeline.GetUserTimeline(userName, 200).ToArray();
        //     tweets.AddRange(receivedTweets);
        //
        //     while (tweets.Count < maxNumberOfTweets && receivedTweets.Length == 200)
        //     {
        //         var oldestTweet = tweets.Min(x => x.Id);
        //         var userTimelineParameter = Timeline.CreateUserTimelineRequestParameter(userName);
        //         userTimelineParameter.MaxId = oldestTweet;
        //         userTimelineParameter.MaximumNumberOfTweetsToRetrieve = 200;
        //
        //         receivedTweets = Timeline.GetUserTimeline(userTimelineParameter).ToArray();
        //         tweets.AddRange(receivedTweets);
        //     }
        //
        //     return tweets.Distinct().ToArray();
        // }
    }
}
