using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockSenti
{
    class PriceInfo
    {
        private double _closePrice;
        private string _marketClose;
        private string _companyCode;
        private string _date;
        private double _priceChange;
        private double _beta;
        private string _performance;
        private bool _isMarketInfo;

        public PriceInfo(string date, double closePrice)
        {
            this._date = date;
            this._closePrice = closePrice;
        }

        public PriceInfo(string date, double closePrice, double priceChange, string companyCode, bool isMarketInfo, double beta)
        {
            this._date = date;
            this._closePrice = closePrice;
            this.PriceChange = priceChange;
            this.CompanyCode = companyCode;
            this._isMarketInfo = isMarketInfo;
            this._beta = beta;
        }

        public double ClosePrice
        {
            get { return _closePrice; }
            set { this._closePrice = value; }
        }

        public double PriceChange
        {
            get { return _priceChange; }
            set { this._priceChange = value; }
        }

        public string Performance
        {
            get { return _performance; }
            set { this._performance = value; }
        }

        public string getPerformance(double marketChange)
        {
            double theoriticalChange = this._beta * marketChange;
            
            if (this.PriceChange > theoriticalChange)
            {
                return "good";
            }
            else 
            {
                return "bad";
            }
        }

        public string getPerformanceWithMargin(double marketChange, double margin)
        {
            double theoriticalChange = this._beta * marketChange;

            if (PriceChange > theoriticalChange && (PriceChange - theoriticalChange) > margin)
            {
                return "good";
            }
            else if (PriceChange > theoriticalChange && (PriceChange - theoriticalChange) < margin)
            {
                return "neutral";
            }
            else
            {
                return "bad";
            }
        }

        public double Beta
        {
            get { return _beta; }
            set { this._beta = value; }
        }

        public bool IsMarketInfo
        {
            get { return _isMarketInfo; }
            set { this._isMarketInfo = value; }
        }

        public string MarketClose
        {
            get { return _marketClose; }
            set { this._marketClose = value; }
        }

        public string CompanyCode
        {
            get { return _companyCode; }
            set { this._companyCode = value; }
        }

        public string Date
        {
            get { return _date; }
            set { this._date = value; }
        }

        public bool isHigherPrice(PriceInfo otherPrice)
        {
            if (this._closePrice > otherPrice.ClosePrice)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool islowerPrice(PriceInfo otherPrice)
        {
            if (this._closePrice < otherPrice.ClosePrice)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
