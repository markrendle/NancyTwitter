using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NancyTwitter
{
    using Nancy;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => View["Index"];
        }
    }
}