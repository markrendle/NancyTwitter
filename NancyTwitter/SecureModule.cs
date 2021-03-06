﻿namespace NancyTwitter
{
    using Nancy;
    using Nancy.Security;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["SecureIndex", Context.CurrentUser];
        }
    }
}