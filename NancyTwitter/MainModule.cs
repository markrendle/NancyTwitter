namespace NancyTwitter
{
    using System.Dynamic;
    using Nancy;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => View["Index"];

            Get["/login"] = _ =>
                                {
                                    dynamic model = new ExpandoObject();
                                    model.ReturnUrl = Request.Query.returnUrl;
                                    return View["Login", model];
                                };
        }
    }
}