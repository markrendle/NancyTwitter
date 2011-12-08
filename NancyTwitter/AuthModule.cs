namespace NancyTwitter
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Responses;
    using OAuth;

    public class AuthModule : NancyModule
    {
        const string ConsumerKey = "";
        const string ConsumerSecret = "";
        private const string TwitterCallback = "http://localhost:3000/auth/twitter_callback";

        public AuthModule(ITwitter twitter) : base("/auth")
        {
            Get["/twitter"] = _ =>
                                  {
                                      string callback = TwitterCallback;
                                      if (Request.Query.returnUrl != null)
                                      {
                                          callback += "?returnUrl=" + ((string) Request.Query.returnUrl).UrlEncode();
                                      }
                                      return new RedirectResponse(twitter.GetAuthorizeUri(ConsumerKey, ConsumerSecret, new Uri(callback)).ToString());
                                  };

            Get["/twitter_callback"] = _ => this.LoginAndRedirect((Guid) twitter.GetUser(ConsumerKey, ConsumerSecret,
                                                                                         Request.Query.oauth_token,
                                                                                         Request.Query.oauth_verifier).
                                                                             UserGuid, DateTime.Today.AddDays(1), Request.Query.returnUrl as string ?? "/");
        }
    }
}