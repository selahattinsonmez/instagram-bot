using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace insta
{
    class Program
    {
        static ChromeDriver chrome = new ChromeDriver(@"CHROME_DRIVER_PATH");

        static void Main(string[] args)
        {
            login();
            Thread.Sleep(2000);
            var linksOfPosts = getLinksOfPosts("https://www.instagram.com/kimola101/");
            var linksOfLocations = getLinksOfLocationsWithAgilityPack(linksOfPosts);
            var locationsInformation = getInformationOfLocations(linksOfLocations);



            foreach(var node in locationsInformation)
            {
                Console.WriteLine(node[0] + "  " + node[1] + "  " + node[2]);
            }
            Console.ReadLine();

        }

        private static List<List<String>> getInformationOfLocations(List<string> linksOfLocations)
        {
            List<List<String>> informations = new List<List<string>>();
            foreach(var link in linksOfLocations)
            {
                chrome.Navigate().GoToUrl(link);
                List<String> node = new List<string>();
                node.Add(chrome.FindElement(By.XPath("//meta[@property='place:location:latitude']")).GetAttribute("content"));
                node.Add(chrome.FindElement(By.XPath("//meta[@property='place:location:longitude']")).GetAttribute("content"));
                var title = chrome.Title;
                node.Add(title.Substring(0, title.IndexOf("on Instagram") - 1));
                informations.Add(node);
            }


            return informations;
        }

        private static List<string> getLinksOfLocationsWithSelenium(List<string> linksOfPosts)
        {
            List<string> linksList = new List<string>() ;
            foreach (var link in linksOfPosts)
            {
                chrome.Navigate().GoToUrl(link);
                String json = chrome.FindElement(By.XPath("//script[@type='application/ld+json']")).GetAttribute("innerHTML");

                if (json.Contains("CollectionPage"))
                {
                    var url = findSubstring(json, json.IndexOf("CollectionPage") + 23);
                    url=url.Replace(@"\","");
                    linksList.Add(url);
                }
            }


            return linksList;
        }
        private static List<string> getLinksOfLocationsWithAgilityPack(List<string> linksOfPosts)
        {
            List<string> linksList = new List<string>();
            HtmlDocument doc;
            var web = new HtmlWeb();

            foreach (var link in linksOfPosts)
            {
                

                doc = web.Load(link);
                String json = doc.DocumentNode.SelectNodes("//script[contains(.,'@context')]")[0].InnerHtml;
                if (json.Contains("CollectionPage"))
                {
                    var url = findSubstring(json, json.IndexOf("CollectionPage") + 23);
                    url = url.Replace(@"\", "");
                    linksList.Add(url);
                }
            }


            return linksList;
        }

        private static string findSubstring(string result, int index)
        {

            int i = 0; Boolean control = true;
            while (control)
            {
                i++;
                if (result[index + i] == '\"')
                {
                    control = false;
                }
            }
            return result.Substring(index, i);

        }



        private static List<string> getLinksOfPosts(string url)
        {
            chrome.Navigate().GoToUrl(url);
            IJavaScriptExecutor jse = (IJavaScriptExecutor)chrome;
            var lengthOfPage = jse.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);var lenOfPage=document.body.scrollHeight;return lenOfPage;");
            var lastCount = lengthOfPage;
            var match = false;
            List<string> linksList = new List<string>();

            while (match==false)
            {
                lastCount = lengthOfPage;
                Thread.Sleep(1000);
                var elements = chrome.FindElementsByClassName("v1Nh3");
                for (int i = 0; i < elements.Count; i++)
                {
                    try
                    {
                        var postUrl = elements[i].FindElement(By.CssSelector("a")).GetAttribute("href");

                        if (!linksList.Contains(postUrl))
                        {
                            linksList.Add(postUrl);

                        }
                    }
                    catch(Exception e)
                    {

                    }


                }
                lengthOfPage = chrome.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);var lenOfPage=document.body.scrollHeight;return lenOfPage;");

                if (lastCount.ToString()[2] == lengthOfPage.ToString()[2])
                    match = true;

            }
            return linksList;
        }

        private static void login()
        {
            chrome.Navigate().GoToUrl("https://instagram.com/accounts/login/");
            var userName = chrome.FindElement(By.Id("react-root")).FindElement(By.Name("username"));
            var pass = chrome.FindElement(By.Id("react-root")).FindElement(By.Name("password"));
            var button = chrome.FindElement(By.Id("react-root")).FindElement(By.TagName("button"));
            userName.SendKeys("YOUR_INSTAGRAM_USERNAME_OR_EMAIL");
            pass.SendKeys("YOUR_INSTAGRAM_PASSWORD");

            button.Click();
        }
    }
}
