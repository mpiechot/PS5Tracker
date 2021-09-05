using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PS5Bot.Websitetracker
{
    public class EuronicsTracker : WebsiteTracker
    {
        public EuronicsTracker() :
            base("Euronics",
                "buybox--button")
        {

        }
    }
}