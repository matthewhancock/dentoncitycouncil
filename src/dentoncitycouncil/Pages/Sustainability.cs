using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace dentoncitycouncil.Pages {
    public class Sustainability : Site.Page {

        public static string GenerateContent() {
            var sb = new StringBuilder();
            var links = JsonConvert.DeserializeObject<List<Site.Link>>(Util.File.LoadToString("Links/Sustainability.json").Result);
            var content = Util.File.LoadToString("Content/Sustainability.txt").Result;

            sb.Append(content.Replace("\r\n", "<br />"));
            sb.Append("<img src=\"/image/Sustainability\" />");
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
                return "sustainability";
            }
        }
        public override string Path {
            get {
                return "Sustainability";
            }
        }
        public override string Header {
            get {
                return "Sustainability";
            }
        }

        public override string Title {
            get {
                return Application.Title + " - Sustainability";
            }
        }

        public override string TitleNav {
            get {
                return "Sustainability";
            }
        }
    }
}
