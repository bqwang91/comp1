using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace StockSenti
{
    class Utility
    {
        public static string getMonthNumber(string month)
        {
            switch (month)
            { 
                case "Jan":
                    return "01";
                case "Feb":
                    return "02";
                case "Mar":
                    return "03";
                case "Apr":
                    return "04";
                case "May":
                    return "05";
                case "Jun":
                    return "06";
                case "Jul":
                    return "07";
                case "Aug":
                    return "08";
                case "Sep":
                    return "09";
                case "Oct":
                    return "10";
                case "Nov":
                    return "11";
                case "Dec":
                    return "12";
                default:
                    return "00";
                break;
            }
        }

        public static string cleanUrlAmp(string url)
        {
            string amp = "&amp;";
            string cleanedUrl = url.Replace(amp, "&");
            return cleanedUrl;

        }

        public static string cleanUrlNewsLink(string url)
        {
            int start = url.IndexOf("/url?q=") + 7;
            int end = url.IndexOf("&sa=");
            int strLength = end - start;
            string cleanedUrl = url.Substring(start, strLength);
            return cleanedUrl;
        }

        public static string removeAllTags(string html)
        {
            html = remvoveHtmlTag(html, "a");
            html = remvoveHtmlTag(html, "small");
            html = remvoveHtmlTag(html, "cite");
            html = remvoveHtmlTag(html, "em");
            html = remvoveHtmlTag(html, "strong");
            html = remvoveHtmlTag(html, "span");
            return html.Trim();
        }
        public static string remvoveHtmlTag(string html, string tagName)
        {
            string tagStart = "<" + tagName;
            string tagEndOne = "</" + tagName + ">";
            string tagEndTwo = "<" + tagName + "/>";

            while (html.Contains(tagStart))
            {
                int start = html.IndexOf(tagStart);
                int end = html.LastIndexOf(tagEndOne);
                
                string subFirstPart;
                if (start == 0)
                {
                    subFirstPart = "";
                }
                else
                {
                    subFirstPart = html.Substring(0, start - 1);
                }
                string subSecondPart = html.Substring(end + tagEndOne.Length,html.Length -end - tagEndOne.Length);
                html = subFirstPart + subSecondPart;
            }

            return html;
        }

        public static string remvoveJavaScriptWarnings(string input)
        {

            string javaScript = "\nPlease enable JavaScript to use My News, My Clippings, My Comments and user settings." + 
                "\nPersonalise your news, save articles to read later and customise settings." + 
                "\nIf you have trouble accessing our login form below, you can go to our." +
                "\nIf you have trouble accessing our login form below, you can go to our.";
            input = input.Substring(javaScript.Length - 1,input.Length - javaScript.Length);
            return input;
        }
    }
}
