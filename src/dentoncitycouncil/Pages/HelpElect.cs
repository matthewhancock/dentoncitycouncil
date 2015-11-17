using System;
using System.Collections.Generic;

namespace dentoncitycouncil.Pages {
    public class HelpElect : Site.Page {

        public static string GenerateContent() {
            var content = Util.File.LoadToString("Content/HelpElect.txt").Result;
            content = content.Replace("\r\n", "<br />");
            content += "<hr />";
            var token_support = Guid.NewGuid().ToString("N");
            //content += $"<div class=\"f\">{GetFormSupport()}{GetFormDonate()}</div>"; 
            content += "<div class=\"tac\">'Thank you for your support. The donation filing period for the 2015 election has closed.</div>";
            content += "<hr />";
            content += "<img src=\"/image/HelpElect\" />";
            return content;
        }

        private static string GetFormDonate() {
            var token = Guid.NewGuid().ToString("N");
            return $"<div class=\"f1 f fcc\"><h2>Donate</h2><div id=\"{Forms.HelpElect.HtmlID.DonateFormContainer}\"><div id=\"f_{token}\" data-action=\"{Forms.HelpElect.Action.Donate}\" class=\"form ib\">" +
                new Site.Form.Field.Textbox(Forms.HelpElect.Keys.Name, "Name", true).Output() +
                new Site.Form.Field.Textbox(Forms.HelpElect.Keys.Email, "Email Address", true, null, null, "email").Output() +
                new Site.Form.Field.CreditCard(Forms.HelpElect.Keys.CreditCard).Output() +
                new Site.Form.Field.RadioSuggest(Forms.HelpElect.Keys.Amount, "Amount", new string[] { "5", "10", "15", "25", "50", "75", "100" }, true).Output() +
                "<br /><div class=\"form_buttons\"><div id=\"f_" + token + "_error\" class=\"error hide\"></div><input type=\"button\" onclick=\"a('f_" + token + "',this)\" value=\"Submit\" /></div></div></div></div>";
        }
        private static string GetFormSupport() {
            var token = Guid.NewGuid().ToString("N");
            return $"<div class=\"f1 f fcc\"><h2>Support</h2><div id=\"{Forms.HelpElect.HtmlID.SupportFormContainer}\"><div id=\"f_{token}\" data-action=\"{Forms.HelpElect.Action.Support}\" class=\"form ib\">" +
                new Site.Form.Field.Textbox(Forms.HelpElect.Keys.Name, "Name", true).Output() +
                new Site.Form.Field.Textbox(Forms.HelpElect.Keys.Email, "Email Address", true, null, null, "email").Output() +
                new Site.Form.Field.Textarea(Forms.HelpElect.Keys.Address, "Address", true).Output() +
                new Site.Form.Field.Checkbox(Forms.HelpElect.Keys.YardSign, "I'd like a yard sign").Output() +
                new Site.Form.Field.Checkbox(Forms.HelpElect.Keys.BumperSticker, "I'd like a magnetic bumper sticker").Output() +
                "<br /><div class=\"form_buttons\"><div id=\"f_" + token + "_error\" class=\"error hide\"></div><input type=\"button\" onclick=\"a('f_" + token + "',this)\" value=\"Submit\" /></div></div></div></div>";
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
                return "help-elect";
            }
        }
        public override string Path {
            get {
                return "Help Elect Josh";
            }
        }
        public override string Header {
            get {
                return "Help Elect Josh";
            }
        }

        public override string Title {
            get {
                return Application.Title + " - Help Elect Josh";
            }
        }

        public override string TitleNav {
            get {
                return "Help Elect Josh";
            }
        }
    }
}
