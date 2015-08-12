using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    /// <summary>
    /// Summary description for Html
    /// </summary>
    public static class Html
    {
        public static class Head
        {
            public class Tag
            {
                public Tag(string TagName, IDictionary<string, string> Attributes)
                {
                    _tag = TagName;
                    _attributes = Attributes;
                }
                private string _tag;
                private IDictionary<string, string> _attributes;
                public virtual string Output()
                {
                    var s = "<" + _tag + " ";
                    foreach (var i in _attributes)
                    {
                        s += i.Key + "=\"" + i.Value + "\" ";
                    }
                    return s + "/>";
                }
                public class Javascript : Tag
                {
                    public Javascript(string Url) : base("script", null)
                    {
                        _url = Url;
                    }
                    private string _url;
                    public override string Output()
                    {
                        return "<script src=\"" + _url + "\" defer></script>";
                    }
                }
            }
		}
		public static async Task WriteOutput(HttpResponse Response, string Title, Head.Tag[] HeadTags, string Body) {
			var sb = new StringBuilder();
			sb.Append("<!DOCTYPE html><head><meta charset=\"utf-8\">");
			if (Title != null) {
				sb.Append("<title>" + Title + "</title>");
			}
			if (HeadTags != null) {
				foreach (var a in HeadTags) {
					sb.Append(a.Output());
				}
			}
			sb.Append("</head><body>");
			sb.Append(Body);
			sb.Append("</body></html>");
			await Response.WriteAsync(sb.ToString());
		}
		public static async Task WriteOutput(HttpResponse Response, string Title, string HeadTags, string Body)
        {
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><head><meta charset=\"utf-8\">");
            if (Title != null)
            {
                sb.Append("<title>" + Title + "</title>");
            }
            if (HeadTags != null)
            {
				sb.Append(HeadTags);
            }
            sb.Append("</head><body>");
            sb.Append(Body);
            sb.Append("</body></html>");
            await Response.WriteAsync(sb.ToString());
        }
        public static async Task WriteOutput(HttpResponse Response, string Title, string Body)
        {
            await WriteOutput(Response, Title, (string) null, Body);
        }
    }
}