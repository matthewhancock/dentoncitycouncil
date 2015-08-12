using System;

namespace Util
{
    public static class Json
    {
		public static string Fix(string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			} else {
				return s.Replace(System.Environment.NewLine, String.Empty).Replace(@"\", @"\\").Replace(@"""", @"\""");
            }
        }
    }
}