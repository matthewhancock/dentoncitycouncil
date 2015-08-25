using System;

namespace dentoncitycouncil.Pages {
    public class Home : Site.Page {

        public static string GenerateContent() {
            var sb = new StringBuilder();
            var links = JsonConvert.DeserializeObject<List<Site.Link>>(Util.File.LoadToString("Links/Home.json").Result);
            var content = Util.File.LoadToString("Content/Home.txt").Result;
            
            sb.Append(content.Replace("\r\n", "<br />"));
            sb.Append("<br /><br />" +  Site.Svg.Signature + "<img src=\"/image/Josh-Denton-Dogs\" />");
            sb.Append("<hr />The links below are to relevant Portsmouth Herald articles and letters to the editor I authored:<br /><br />");
            foreach (var i in links) {
                sb.Append($"<a href=\"{i.Url}\" target=\"_blank\">{i.Title}</a><br />{i.Date.ToString("MMMM dd, yyyy")}");
                if (i.Description != null) {
                    sb.Append("<br />" + i.Description);
                }
                sb.Append("<br /><br />");
            }
            return sb.ToString();
        }

        private string _content;
        public override string Content {
            get {
                if (_content == null) {
                    _content = GenerateContent();
                }
                return _content;
            }
        }

        public override string Description {
            get {
                return "";
            }
        }

        public override string Key {
            get {
                return "home";
            }
        }
        public override string Path {
            get {
                return string.Empty;
            }
        }
        public override string Header {
            get {
                return "Home";
            }
        }

        public override string Title {
            get {
                return Application.Title;
            }
        }

        public override string TitleNav {
            get {
                return "Home";
            }
        }
    }
}
