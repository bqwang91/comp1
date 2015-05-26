using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data.SQLite;
using System.Data.Common;
using HtmlAgilityPack;

namespace StockSenti
{
    class WebCrawler : Crawler
    {
        Dictionary<string, string> sourceArticleMap = new Dictionary<string, string>()
                {
                    {"Sydney Morning Herald","articleBody"},
                    {"Sky News Australia", "row article-body free-text"},
                    {"ABC Online", "article section"},
                    {"Motley Fool Australia", "full_content"},
                    {"The Australian Financial Review","cq-article-content-paras section"}
                };
        /*
         * Source: Main: The Australian, Sydney Morning Herald, ABC Online, The Australian Financial Review, Sky News Australia
         * Source: Other: Motley Fool Australia, Australian Mining, SmartCompany.com.au, 
         * Seed: https://www.google.com.au/search?hl=en&gl=au&tbm=nws&authuser=0&q=bhp&oq=bhp&gs_l=news-cc.3...643.643.0.664.1.1.0.0.0.0.0.0..0.0...0.0...1ac.1.
         * Each news is represented by a <li class="g">, with info store in a <div class="_cnc">
         * Title: the text in <a> tag with href of the link to the article in <h3> tag inside the <div> above 
         * Source: <div class = "slp"> <span class="_tQb _IId">Source Name</span>
         * <span class="f nsa _uQb">Date</span>
         * 
         * User ID selector: id = pnnext to navigate to the next page
         * <td class="b navend">
         * <a class="pn" href="\partial link" id="pnnext"><span class="csb gbil ch">Next</span></a></td>
         * 
         * 
         * */

        public void crawlGoogleNewsArticles(List<Article> articleContainer, int limit)
        {
            int count = 0;

            while (count < limit)
            {
                String seedContent = this.getUrlContent(this._seedUrl);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(seedContent);

                HtmlNode nextPageNode = doc.DocumentNode.SelectSingleNode("//td[@class='b' and @style='text-align:left']");

                HtmlNode[] ariticleNodes = doc.DocumentNode.SelectNodes("//li[@class='g']").ToArray();

                foreach (HtmlNode node in ariticleNodes)
                {
                    HtmlNode newsNode = node.SelectSingleNode("table[1]").SelectSingleNode("tr[1]");
                    HtmlNode titleSourceNode = newsNode.SelectSingleNode("td[1]");
                    HtmlNode titleLinkNode = titleSourceNode.SelectSingleNode("h3[1]").SelectSingleNode("a[1]");

                    HtmlNode sourceDateNode = titleSourceNode.SelectSingleNode("div[1]").SelectSingleNode("span[1]");

                    string sourceDate = sourceDateNode.InnerHtml;

                    string[] sourceDateSplit = sourceDate.Split(new string[] { " - " }, StringSplitOptions.None);

                    string title = titleLinkNode.InnerHtml;
                    string source = sourceDateSplit[0];
                    string date = sourceDateSplit[1];
                    string articleUrl = Utility.cleanUrlAmp(titleLinkNode.Attributes["href"].Value);

                    if (sourceArticleMap.ContainsKey(source))
                    {
                        Article newArticle = new Article();
                        newArticle.Title = title;
                        newArticle.Source = source;
                        newArticle.Date = "2015-05-16";
                        newArticle.Url = articleUrl;
                        newArticle.CompanyCode = "BHP";

                        string crawledContent = crawlNewsContent(Utility.cleanUrlNewsLink(articleUrl), source, sourceArticleMap);
                        string cleanedContent = Utility.removeAllTags(crawledContent);

                        newArticle.Content = cleanedContent;

                        articleContainer.Add(newArticle);
                        count++;
                    }
                    else
                    {
                        continue;
                    }

                }

                HtmlNode aNode = nextPageNode.SelectSingleNode("//a[1]");
                string aHtmlText = nextPageNode.InnerHtml;
                string cleanedAHtmlText = Utility.cleanUrlAmp(aHtmlText);
                int start = cleanedAHtmlText.IndexOf("/search");
                int end = cleanedAHtmlText.IndexOf("sa=N");
                int strLength = end + 4 - start;
                string partialUrl = cleanedAHtmlText.Substring(start, strLength);

                if (!partialUrl.Contains("https://"))
                    this._seedUrl = "https://www.google.com.au" + partialUrl;
                else
                    this._seedUrl = partialUrl;
                if (!partialUrl.Contains("sa=N"))
                    this._seedUrl = this._seedUrl + partialUrl;
            }
        }

        public void crawlSmhNewsArticles(List<Article> articleContainer, string companyCode, DbConnection dbConn,int limit)
        {
            int count = 0;

            while (count < limit)
            {
                String seedContent = this.getUrlContent(this._seedUrl);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(seedContent);

                HtmlNode nextPageNode = doc.DocumentNode.SelectSingleNode("//li[@class='next' and @rel='next']");

                HtmlNode[] ariticleNodes = doc.DocumentNode.SelectNodes("//div[@class='cT-searchResult cfix']").ToArray();

                foreach (HtmlNode node in ariticleNodes)
                {
                    HtmlNode h3Node = node.SelectSingleNode("h3[1]");
                    HtmlNode newsNode = h3Node.SelectSingleNode("a[1]");
                    HtmlNode titleSourceNode = node.SelectSingleNode("p[2]");

                    HtmlNode sourceDateNode = titleSourceNode.SelectSingleNode("cite[1]");

                    string sourceDate = sourceDateNode.InnerHtml;

                    string[] sourceDateSplit = sourceDate.Split(new string[] { " | " }, StringSplitOptions.None);

                    string title = newsNode.InnerHtml;
                    string source = "Sydney Morning Herald";
                    string dateRaw = sourceDateSplit[sourceDateSplit.Length - 1];

                    string[] dateTokens = dateRaw.Split(new string[] { " " }, StringSplitOptions.None);

                    string date = "";
                    if (dateTokens.Length == 6)
                    {
                        string day = dateTokens[2];
                        string month = Utility.getMonthNumber(dateTokens[1]);
                        string year = dateTokens[5];
                        date = year + "-" + month + "-" + day;
                    }
                    else 
                    {
                        continue;
                    }

                    string articleUrl = Utility.cleanUrlAmp(newsNode.Attributes["href"].Value);

                    Article newArticle = new Article();
                    newArticle.Title = title;
                    newArticle.Source = source;
                    newArticle.Date = date;
                    newArticle.Url = articleUrl;
                    newArticle.CompanyCode = companyCode;

                    newArticle.Classification = DbOperation.getStockPerformance(dbConn,newArticle.CompanyCode,newArticle.Date);
                    

                    string crawledContent = crawlNewsContent(articleUrl, source, sourceArticleMap);
                    if (crawledContent.Equals("404"))
                    {
                        continue;
                    }
                    string cleanedCrawledContent = Utility.remvoveJavaScriptWarnings(crawledContent);

                    newArticle.Content = cleanedCrawledContent;
                    articleContainer.Add(newArticle);
                    count++;
                }

                HtmlNode aNode = nextPageNode.SelectSingleNode("a[1]");
                string nextPageLink = Utility.cleanUrlAmp(aNode.Attributes["href"].Value);
                this._seedUrl = nextPageLink;
            }

        }

        public string crawlNewsContent(string url, string source, Dictionary<string, string> sourceArticleMap)
        {
            String webContent = this.getUrlContent(url);
            if (webContent.Equals("404"))
            {
                return webContent;
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(webContent);
            string className = "";

            if (sourceArticleMap.ContainsKey(source))
            {
                sourceArticleMap.TryGetValue(source, out className);
            }

            HtmlNode[] paragraphNodes;

            if (source.Equals("Sky News Australia"))
            {
                HtmlNode divNode = doc.DocumentNode.SelectSingleNode("//div" + "[@class='" + className + "']");
                HtmlNode subDivNode = divNode.SelectSingleNode("//div[1]");
                paragraphNodes = subDivNode.SelectNodes("//p").ToArray();
            }
            else if (source.Equals("Motley Fool Australia"))
            {
                paragraphNodes = doc.DocumentNode.SelectSingleNode("//div" + "[@id='" + className + "']").SelectNodes("//p").ToArray();
            }
            else
            {
                paragraphNodes = doc.DocumentNode.SelectSingleNode("//div" + "[@class='" + className + "']").SelectNodes("//p").ToArray();
            }

            string content = "";
            foreach (HtmlNode paraNode in paragraphNodes)
            {
                string cleanedHtml = paraNode.InnerHtml.Replace("&nbsp;", "");
                cleanedHtml = cleanedHtml.Replace("&amp;", "");
                content = content + "\n" + Utility.removeAllTags(cleanedHtml);
            }
            content.TrimStart('\n');
            return content;

        }


    }
}
