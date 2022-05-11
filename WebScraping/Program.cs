using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebScraping
{
    class Program
    {
        private const string HTMLFILEPATH = "html_file_01.txt";

        static void Main(string[] args)
        {
            Scrap(GetDocument());
        }

        private static HtmlDocument GetDocument()
        {
            HtmlDocument doc = new HtmlDocument();

            string content = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HTMLFILEPATH));

            doc.LoadHtml(content);
            return doc;
        }

        private static void Scrap(HtmlDocument doc)
        {
            List<Item> items = new List<Item>();

            var nodes = doc.DocumentNode.SelectNodes("//div")
                .Where(d => d.Attributes["class"] != null
                && d.Attributes["class"].Value == "item");

            foreach (var node in nodes)
            {

                var titles = from el in node.Descendants() where el.Name == "h4" select HttpUtility.HtmlDecode(el.InnerText);
                var prices = from el in node.Descendants() where el.Name == "p" && el.Attributes["class"].Value == "price" select el.FirstChild;
                var rating = from el in node.GetAttributes() where el.Name != null && el.Name == "rating" select el.Value;
              
                for (int i = 0; i < titles.Count(); i++)
                {

                    float rat = 0.0f;
                    float.TryParse(rating.ElementAt(i), out rat);
                    rat = rat > 5 ? rat / 2 : rat;

                    items.Add(new Item
                    {
                        productName = titles.ElementAt(i),
                        price = prices.ElementAt(i).FirstChild.InnerText.Trim('$'),
                        rating = rat
                    });
                }
            }

            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    class Item
    {
        public string productName { get; set; }
        public string price { get; set; }
        public float rating { get; set; }
    }
}
