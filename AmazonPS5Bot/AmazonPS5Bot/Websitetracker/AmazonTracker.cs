using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PS5Bot.Websitetracker
{
    public class AmazonTracker : WebsiteTracker
    {
        public AmazonTracker() :
            base("Amazon",
                "add-to-cart-button")
        {
            
        }
        
        public override void CheckWebsite()
        {
            foreach (string url in shopDictionary.Keys)
            {
                HtmlDocument doc = GetWebsiteContent(url);
                if (doc == null)
                {
                    continue;
                }
                try
                {
                    //Console.WriteLine("Test");
                    if(doc.DocumentNode.SelectNodes($"//input[(@id='{CART_BUTTON_ID}' or @class='{CART_BUTTON_ID}') and not(@disabled)]").Any())
                    {
                        Console.WriteLine($"{DateTime.Now} found at {shopName}");
                        if (LastActiveSomeTimeAgo(shopDictionary[url]))
                        {
                            Console.Write("!");
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
                    //Console.WriteLine("Error: " + e.Message);
                    continue;
                }
            }
        }
    }
}
