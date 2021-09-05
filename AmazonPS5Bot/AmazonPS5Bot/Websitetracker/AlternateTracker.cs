using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PS5Bot.Websitetracker
{
    public class AlternateTracker : WebsiteTracker
    {
        public AlternateTracker() :
            base("Alternate",
                "add-to-cart-form:add-to-cart-section")
        {

        }
        
        public override void CheckWebsite()
        {
            foreach (string url in shopDictionary.Keys)
            {
                //Console.WriteLine("Check: " + url);
                HtmlDocument doc = GetWebsiteContent(url);
                if (doc == null)
                {
                    continue;
                }
                try
                {
                    if(doc.DocumentNode.SelectNodes($"//div[@id='{CART_BUTTON_ID}' and not(@disabled)]").Any())
                    {
                        Console.WriteLine($"{DateTime.Now} found at {shopName}");
                        if (LastActiveSomeTimeAgo(shopDictionary[url]))
                        {
                            shopDictionary[url] = DateTime.Now;
                            InvokeInStockEvent(this, new Product()
                            {
                                Shop = shopName,
                                Name = "PS5",
                                Price = "ca. 500",
                                URL = url
                            });
                        }
                    }
                }
                catch(Exception e)
                {
                    continue;
                }
            }
        }
    }
}
