namespace NancyTwitter.OAuth
{
    using System;

    public interface ITwitter
    {
        Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback);
        TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier);
    }
}