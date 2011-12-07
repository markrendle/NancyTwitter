namespace NancyTwitter
{
    using System;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using OAuth;

    public class UserMapper : IUserMapper
    {
        public IUserIdentity GetUserFromIdentifier(Guid identifier)
        {
            return new TwitterUser {UserGuid = identifier, UserName = identifier.ToString("N")};
        }
    }
}