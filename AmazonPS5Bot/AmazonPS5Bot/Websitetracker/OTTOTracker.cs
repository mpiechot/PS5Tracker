using System;
using System.Collections.Generic;
using System.Text;

namespace PS5Bot.Websitetracker
{
    public class OTTOTracker : WebsiteTracker
    {
        public OTTOTracker(string url) :
            base( "OTTO",
                "//li[@class='promo_articlelist--article']")
        {

        }
    }
}
