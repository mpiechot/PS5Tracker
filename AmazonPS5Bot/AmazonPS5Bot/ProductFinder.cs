using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PS5Bot
{
    public class ProductFinder
    {
        public static List<string> searchPatterns = new List<string>();

        public static bool ProductInside(string html)
        {
            foreach(string pattern in searchPatterns)
            {
                if(Regex.IsMatch(html.ToLower(), pattern.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
