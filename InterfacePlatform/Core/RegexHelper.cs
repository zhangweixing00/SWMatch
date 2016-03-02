using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace InterfacePlatform.Core
{
    public class RegexHelper
    {
        public static string GetText(string input, string regex, string key)
        {
            Match match = Regex.Match(input, regex);
            if (match.Success)
            {
                return match.Groups[key].Value;
            }
            return "";
        }
    }
}