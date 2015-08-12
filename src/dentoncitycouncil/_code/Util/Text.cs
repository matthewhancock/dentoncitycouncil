using System;

namespace Util
{
    /// <summary>
    /// Summary description for Text
    /// </summary>
    public static class Text
    {
        internal static string StripHtml(string HtmlString)
        {
            return System.Text.RegularExpressions.Regex.Replace(HtmlString, "<(.|\n)*?>", String.Empty).Replace("&nbsp;", " ");
        }
    }
}