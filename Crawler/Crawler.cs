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
    class Crawler
    {
        protected string _seedUrl;

        public string SeedUrl
        {
            get { return _seedUrl; }
            set { this._seedUrl = value; }
        }

        protected String getUrlContent(String url)
        {
            StringBuilder sb = new StringBuilder();
            // buffer
            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                return "404";
            }

            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // read data into buffer
                count = resStream.Read(buf, 0, buf.Length);

                // if we read any data
                if (count != 0)
                {
                    // put into a temporary String
                    tempString = Encoding.UTF8.GetString(buf, 0, count);

                    // append to the StringBuilder
                    sb.Append(tempString);
                }
            }
            while (count > 0);

            return sb.ToString();
        }
        
        

    }
}
