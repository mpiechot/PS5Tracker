using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PS5Bot.Websitetracker
{
    public abstract class WebsiteTracker
    {
        protected Dictionary<string, DateTime> shopDictionary = new Dictionary<string, DateTime>();
        protected string CART_BUTTON_ID;
        protected string shopName;
        private string shopFolder = @"D:\GitHub\PS5Tracker\AmazonPS5Bot";

        protected string url;
        public event EventHandler<Product> InStockEvent;

        public WebsiteTracker(string shopName, string cartButtonId)
        {
            this.shopName = shopName;
            CART_BUTTON_ID = cartButtonId;
            FillShopDictionary();
        }

        protected bool LastActiveSomeTimeAgo(DateTime shopTime)
        {
            //Console.WriteLine("Time: " + DateTime.Now.Subtract(TimeSpan.FromHours(2)));
            //Console.WriteLine("Shop-time: "  + shopTime);
            return DateTime.Now.Subtract(TimeSpan.FromMinutes(45)) >= shopTime;
        }

        protected void FillShopDictionary()
        {
            if (File.Exists(Path.Combine(shopFolder,shopName+".shops")))
            {
                string[] shops = File.ReadAllLines($"{shopFolder}/{shopName}.shops");
                foreach (string shop in shops)
                {
                    shopDictionary.Add(shop,DateTime.MinValue);
                }
            }
            else
            {
                Console.WriteLine($"Shopfile {Path.Combine(shopFolder,shopName+".shops")} not found!");
            }
        }
        
        public virtual void CheckWebsite()
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
                    if(doc.DocumentNode.SelectNodes($"//button[(@id='{CART_BUTTON_ID}' or @class='{CART_BUTTON_ID}') and not(@disabled)]").Any())
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
                    continue;
                }
            }
            //Console.WriteLine(shopName + " checked!");
           
            
            
            // HtmlDocument doc = GetWebsiteContent(url);
            //
            // var productNodes = doc.DocumentNode.SelectNodes(PRODUCT_XPATH).Where(x => ProductFinder.ProductInside(x.InnerHtml));
            //
            // Console.WriteLine("Products: " + productNodes.Count());
            // foreach (var productNode in productNodes)
            // {
            //     HtmlDocument productdoc = new HtmlAgilityPack.HtmlDocument();
            //     productdoc.LoadHtml(productNode.InnerHtml);
            //     var name = productdoc.DocumentNode.SelectSingleNode(PRODUCT_NAME_XPATH)?.InnerText;
            //     if (name == null || name.Length == 0)
            //     {
            //         continue;
            //     }
            //     var inStock = productdoc.DocumentNode.SelectSingleNode(IN_STOCK_XPATH) == null;
            //     if (inStock && (!shopDictionary.ContainsKey(name) || !shopDictionary[name]))
            //     {
            //         var price = productdoc.DocumentNode.SelectSingleNode(PRICE_XPATH)?.InnerText;
            //         //Console.WriteLine("InStock? " + inStock);
            //         //Console.WriteLine("-Name: " + name);
            //         //Console.WriteLine("-Price: " + price);
            //         InvokeInStockEvent(this, new Product()
            //         {
            //             Shop = shopName,
            //             Name = name,
            //             Price = price,
            //             URL = url
            //         });
            //         shopDictionary[name] = true;
            //     }
            //     else if (!inStock)
            //     {
            //         shopDictionary[name] = false;
            //     }
            // }
        }
        protected HtmlDocument GetWebsiteContent(string url)
        {
            try
            {
                CookieContainer cookieJar = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookieJar;
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36 OPR/74.0.3911.107";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string htmlString;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    htmlString = reader.ReadToEnd();
                }
                HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlString);
                return doc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void InvokeInStockEvent(object sender, Product product)
        {
            InStockEvent?.Invoke(sender,product);
        }
    }
}
