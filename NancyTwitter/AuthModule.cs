namespace NancyTwitter
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Responses;
    using OAuth;

    public class AuthModule : NancyModule
    {
        const string ConsumerKey = "62LkhQSYQxPbY1oKgbv0w";
        const string ConsumerSecret = "NyIi7vkNxhHAXTjGf751Gx8HBeJTeRNQf4tUUBMc";
        private static readonly Uri TwitterCallback = new Uri("http://localhost:3000/auth/twitter_callback");

        public AuthModule(ITwitter twitter) : base("/auth")
        {
            Get["/twitter"] = _ => new RedirectResponse(twitter.GetAuthorizeUri(ConsumerKey, ConsumerSecret, TwitterCallback).ToString());

            Get["/twitter_callback"] = _ => this.LoginAndRedirect((Guid)twitter.GetUser(ConsumerKey, ConsumerSecret,
                                                            Request.Query.oauth_token,
                                                            Request.Query.oauth_verifier).UserGuid, DateTime.Today.AddDays(1), "/secure");
        }
    }
}