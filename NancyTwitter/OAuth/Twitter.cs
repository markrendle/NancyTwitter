namespace NancyTwitter.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class Twitter : ITwitter
    {
        public Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback)
        {
            return
                new Uri("https://api.twitter.com/oauth/authorize?oauth_token=" +
                        GetOAuthToken(consumerKey, consumerSecret, callback));
        }

        public TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier)
        {
            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret, oauthToken)
                                      {
                                          {OAuthParameter.Verifier, oauthVerifier}
                                      };
            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString(GetAccessTokenUri(), oauthParameters);
            }
            if (response == null) throw new InvalidOperationException("That didn't work then.");

            Dictionary<string, string> values =
                    response.Split('&').Select(section => section.Split('=')).ToDictionary(
                        bits => bits[0], bits => bits[1]);

            return new TwitterUser
                       {
                           Token = values["oauth_token"],
                           TokenSecret = values["oauth_token_secret"],
                           UserId = long.Parse(values["user_id"]),
                           ScreenName = values["screen_name"]
                       };
        }

        private Uri GetRequestTokenUri(Uri callback)
        {
            return
                new Uri("https://api.twitter.com/oauth/request_token?oauth_callback=" + callback.ToString().UrlEncode());
        }

        private Uri GetAccessTokenUri()
        {
            return new Uri("https://api.twitter.com/oauth/access_token");
        }

        private string GetOAuthToken(string consumerKey, string consumerSecret, Uri callback)
        {
            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret)
                                      {
                                          {OAuthParameter.Callback, callback.ToString()},
                                      };

            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.UploadString(GetRequestTokenUri(callback), string.Empty, oauthParameters);
            }

            if (response == null) throw new InvalidOperationException("That didn't work then.");

            Dictionary<string, string> values =
                response.Split('&').Select(section => section.Split('=')).ToDictionary(
                    bits => bits[0], bits => bits[1]);
            return values["oauth_token"];
        }
    }
}