using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Util.Http;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Configuration;
using Util.AspNet;

namespace dentoncitycouncil {
    public class Startup {
        private IConfiguration _config;

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env) {
            Application.LoadFromEnvironment(env);

            // load config
            _config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            // load local config file if no environment variables
            var envn = _config.Get("env") ?? "local";
            if (envn == "local") {
                _config = new ConfigurationBuilder().AddJsonFile(env.ApplicationBasePath + "/config.local.json").Build();
            }
            Application.LoadFromConfig(_config);

            app.ForceHttps();
            app.Run(ProcessRequestAsync);
        }

        public async Task ProcessRequestAsync(HttpContext Context) {
            var rq = Context.Request;
            var rs = Context.Response;

            var path = rq.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var pathc = path.Length;

            if (rq.Method == "POST") {
                if (pathc == 1) {
                    await Handlers.Post.ProcessRequestAsync(Context, path[0]);
                }
            } else {
                if (pathc > 0) {
                    switch (path[0]) {
                        case "css":
                            await Css(rs);
                            break;
                        case "js":
                            await Javascript(rs);
                            break;
                        case "image":
                            await Image(rs, path[1]);
                            break;
                        case "json":
                            if (pathc == 2) {
                                await OutputPageJson(rs, path[1]);
                            } else if (pathc > 2) {
                                await OutputPageJson(rs, path[1]);
                            } else {
                                await Error.FileNotFound(rs);
                            }
                            break;
                        default:
                            if (pathc == 1) {
                                await OutputPage(rs, path[0]);
                            } else if (pathc > 1) {
                                await OutputPage(rs, path[0], path.Skip(1).ToArray());
                            } else {
                                await Error.FileNotFound(rs);
                            }
                            break;
                    }
                } else {
                    await OutputPage(rs, string.Empty);
                }
            }
        }

        private static class Pages {
            public static Site.Page Home = new dentoncitycouncil.Pages.Home();
            public static Site.Page Bio = new dentoncitycouncil.Pages.Bio();
            public static Site.Page Campaign2013 = new dentoncitycouncil.Pages.Campaign2013();
            public static Site.Page HelpElect = new dentoncitycouncil.Pages.HelpElect();
            public static Site.Page Sustainability = new dentoncitycouncil.Pages.Sustainability();
        }

        private static string tags = (new Util.Html.Head.Tag("link", new Dictionary<string, string> { { "rel", "stylesheet" }, { "type", "text/css" }, { "href", "/css?v2" } })).Output() +
            (new Util.Html.Head.Tag("link", new Dictionary<string, string> { { "rel", "stylesheet" }, { "type", "text/css" }, { "href", "//cloud.typography.com/607958/780628/css/fonts.css" } })).Output() +
            (new Util.Html.Head.Tag.Javascript("/js")).Output() + (new Util.Html.Head.Tag.Javascript("//platform.twitter.com/widgets.js")).Output() + (new Util.Html.Head.Tag.Javascript("//connect.facebook.net/en_US/sdk.js#xfbml=1&appId=175259985884771&version=v2.0")).Output();
        private static string body_start = $"<div id=\"c\"><header id=\"h\"><section id=\"h_section1\"><div id=\"h-logo\">{Site.Svg.JoshDenton}</div></section><section id=\"h_section2\"><div id=\"h-logo2\">{Site.Svg.CityCouncil}</div></section><hr id=\"h_hr\" /></header><nav id=\"n\" data-key=\"";
        private static string body_mid = "\">" + Pages.Home.NavLink + Pages.Bio.NavLink + Pages.Campaign2013.NavLink + Pages.Sustainability.NavLink + Pages.HelpElect.NavLink + "</nav><main id=\"m\"><section id=\"content\">";
        private static string body_end = $"</section></main><footer id=\"f\"><aside id=\"social\"><a href=\"https://twitter.com/Denton4PortCity\" class=\"twitter-follow-button\" data-show-count=\"false\" data-size=\"large\" data-dnt=\"true\">Follow @Denton4PortCity</a><div class=\"fb-like-box\" data-href=\"https://www.facebook.com/DentonForCityCouncil\" data-colorscheme=\"light\" data-show-faces=\"false\" data-header=\"false\" data-stream=\"false\" data-show-border=\"false\"></div></aside><div id=\"f_disclaimer\">Paid for by Josh Denton for City Council</div><span>Candidate's Note: All views are Josh's and not those of his current or former employers.</span><hr /></footer></div>";
        private async Task OutputPage(HttpResponse Response, string Path, string[] Parameters = null) {
            Site.Page page = null;

            if (Path == Pages.Home.Path) {
                page = Pages.Home;
            } else if (Path == Pages.Bio.Path) {
                page = Pages.Bio;
            } else if (Path == Pages.Campaign2013.Path) {
                page = Pages.Campaign2013;
            } else if (Path == Pages.HelpElect.Path) {
                page = Pages.HelpElect;
            } else if (Path == Pages.Sustainability.Path) {
                page = Pages.Sustainability;
            }

            if (page == null) {
                await Error.FileNotFound(Response);
            } else {
                await Util.Html.WriteOutput(Response, page.Title, tags + "<meta name=\"description\" content=\"" + page.Description + "\">", body_start + page.Key + body_mid + page.Content + body_end);
            }
        }
        private async Task OutputPageJson(HttpResponse Response, string Key) {
            if (Key == Pages.Home.Key) {
                await Json(Response, Pages.Home);
            } else if (Key == Pages.Bio.Key) {
                await Json(Response, Pages.Bio);
            } else if (Key == Pages.Campaign2013.Key) {
                await Json(Response, Pages.Campaign2013);
            } else if (Key == Pages.HelpElect.Key) {
                await Json(Response, Pages.HelpElect);
            } else if (Key == Pages.Sustainability.Key) {
                await Json(Response, Pages.Sustainability);
            }
        }

        private static string _css, _javascript = null;
        private static Dictionary<string, byte[]> _images = new Dictionary<string, byte[]>();
        private async Task Css(HttpResponse Response) {
            Response.Headers.Add(Headers.Cache, Headers.Values.Cache);
            Response.ContentType = "text/css";

            if (_css == null) {
                _css = await Util.File.LoadToString("_files/css/this.css");
            }
            await Response.WriteAsync(_css);
        }
        private async Task Javascript(HttpResponse Response) {
            Response.Headers.Add(Headers.Cache, Headers.Values.Cache);
            Response.ContentType = "text/javascript";

            if (_javascript == null) {
                _javascript = await Util.File.LoadToString("_files/js/this.js");
            }
            await Response.WriteAsync(_javascript);
        }
        private async Task Image(HttpResponse Response, string Path) {
            Response.Headers.Add(Headers.Cache, Headers.Values.Cache);
            Response.ContentType = "image/jpeg";

            byte[] file = null;
            if (!_images.ContainsKey(Path)) {
                try {
                    file = await Util.File.LoadToBuffer($"_files/images/{Path}.jpg");
                    _images.Add(Path, file);
                } catch { }
            } else {
                file = _images[Path];
            }
            if (file != null) {
                await Response.Body.WriteAsync(file, 0, file.Length);
            } else {
                await Error.FileNotFound(Response);
            }
        }
        private async Task Json(HttpResponse Response, Site.Page Page) {
            Response.ContentType = "text/javascript";
            var s = @"{""title"":""" + Util.Json.Fix(Page.Title) + @""",""description"":""" + Util.Json.Fix(Page.Description) + @""",""header"":""" + Util.Json.Fix(Page.Header) + @""",""key"":""" + Page.Key + @""",""content"":""" + Util.Json.Fix(Page.Content) + @"""}";
            await Response.WriteAsync(s);
        }

        private static class Error {
            public static async Task FileNotFound(HttpResponse Response) {
                Response.StatusCode = 404;
                Response.Body.Close();
            }
        }
    }
}
