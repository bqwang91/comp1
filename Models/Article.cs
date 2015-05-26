using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockSenti
{
    class Article
    {
        private string _companyName;
        private string _companyCode;
        private string _title;
        private string _id;
        private string _source;
        private string _date;
        private string _content;
        private string _url;
        private string _classification;
        private Boolean _isValid = false;

        public string CompanyName
        {
            get { return this._companyName; }
            set { this._companyName = value; }
        }

        public string Classification
        {
            get { return this._classification; }
            set { this._classification = value; }
        }

        public string CompanyCode
        {
            get { return this._companyCode; }
            set { this._companyCode = value; }
        }

        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Source
        {
            get { return this._source; }
            set { this._source = value; }
        }

        public string Date
        {
            get { return this._date; }
            set { this._date = value; }
        }

        public string Content
        {
            get { return this._content; }
            set { this._content = value; }
        }

        public string Url
        {
            get { return this._url; }
            set { this._url = value; }
        }

        public bool IsValid
        {
            get { return this._isValid; }
            set { this._isValid = value; }
        }

    }

}
