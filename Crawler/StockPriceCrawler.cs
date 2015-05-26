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
    class StockPriceCrawler : Crawler
    {
        private string _startDate;
        private string _endDate;

        public string StartDate
        {
            get { return this._startDate; }
            set { this._startDate = value; }
        }

        public string EndDate
        {
            get { return this._endDate; }
            set { this._endDate = value; }
        }

        public void crawlStockPrices(List<PriceInfo> priceInfoContainer,string companyCode,DbConnection dbConn)
        {
            bool isEnd = false;

            while(!isEnd)
            {
                string seedContent = getUrlContent(this._seedUrl);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(seedContent);

                HtmlNode nextPageNode = doc.DocumentNode.SelectSingleNode("//a[@rel='next']");
                if (nextPageNode == null)
                {
                    isEnd = true;
                }

                HtmlNode dataTableNode = doc.DocumentNode.SelectSingleNode("//table[@class='yfnc_datamodoutline1']");
                HtmlNode bodyTrNode = dataTableNode.SelectSingleNode("tr[1]");
                HtmlNode trTdNode = bodyTrNode.SelectSingleNode("td[1]");
                HtmlNode tdTableNode = trTdNode.SelectSingleNode("table[1]");

                HtmlNode[] priceInfoNodes = tdTableNode.SelectNodes("tr").ToArray();

                foreach (HtmlNode node in priceInfoNodes)
                {
                    string date = "";
                    double closePrice = 0.0;
                    double change = 0.0;
                   
                    if(node.SelectNodes("td") != null)
                    {
                        HtmlNode[] tdNodes = node.SelectNodes("td").ToArray();

                        if(tdNodes.Count()  == 7)
                        {
                            string[] dateTokens = tdNodes[0].InnerHtml.Split(new string[] { " " }, StringSplitOptions.None);

                            string month = Utility.getMonthNumber(dateTokens[1]);
                            date = dateTokens[2] + "-" + month + "-" + dateTokens[0] ;
                            closePrice = double.Parse(tdNodes[4].InnerHtml);
                            int priceInfoCount = priceInfoContainer.Count;
                            if (priceInfoCount > 0)
                            { 
                                double prevClosePrice = priceInfoContainer[priceInfoCount - 1].ClosePrice;
                                double priceChange =  prevClosePrice - closePrice; //the price are listed in anti chronological order
                                double percentage = priceChange / closePrice;
                                change = Math.Round(percentage,4);
                                priceInfoContainer[priceInfoCount - 1].PriceChange = change;

                                string markePriceString = DbOperation.getMarketPriceChange(dbConn, priceInfoContainer[priceInfoCount - 1].Date);

                                double marketPrice = 0.0;
                                if (!markePriceString.Equals("null"))
                                {
                                    marketPrice = double.Parse(markePriceString);
                                }
                                priceInfoContainer[priceInfoCount - 1].Performance = priceInfoContainer[priceInfoCount - 1].getPerformance(marketPrice);
                            }
                        }
                        else
                            continue;
                    }
                    else
                    {
                        continue;
                    }

                    PriceInfo newDayPriceInfo = new PriceInfo(date,closePrice,change,companyCode,false,1.2);
                    priceInfoContainer.Add(newDayPriceInfo);

                }

                if (nextPageNode != null)
                {
                    string nextPageLink = nextPageNode.Attributes["href"].Value;
                    string cleanedNextLink = Utility.cleanUrlAmp(nextPageLink);
                    if (!cleanedNextLink.Contains("https://"))
                        this._seedUrl = "https://au.finance.yahoo.com" + cleanedNextLink;
                    else
                        this._seedUrl = cleanedNextLink;

                }

            }
        }

    }
}
