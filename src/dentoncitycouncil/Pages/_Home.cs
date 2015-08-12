using System;

namespace dentoncitycouncil.Pages {
    public class Home : Site.Page {

        public static string GenerateContent() {
            var content = Util.File.LoadToString("Content/Home.txt").Result;
            return content.Replace("\r\n", "<br />") + "<br /><br />" + Site.Svg.Signature + "<img src=\"/image/Josh-Denton-Dogs\" />"; ;
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
